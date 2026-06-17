using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ApiHrm.Domains.Entities;
using ApiHrm.Infrastructures.Persistence;
using Api.Contracts.CashAdvance;
using Applications.Exceptions;
using Applications.Interfaces;

namespace Applications.Services
{
    public class CashAdvanceService : ICashAdvanceService
    {
        // Theoretical daily rate used to compute the employee's earned value
        // for the current cutoff period when no contract rate is stored yet.
        private const decimal DefaultDailyRate = 500m;

        private readonly hrmAppDbContext _context;
        private readonly IMapper _mapper;

        public CashAdvanceService(hrmAppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CashAdvanceReadDto> CreateAsync(CashAdvanceCreateDto dto)
        {
            var employeeExists = await _context.Employees.AnyAsync(e => e.EmployeeId == dto.Employee_Id);
            if (!employeeExists)
                throw new BusinessValidationException($"Employee with ID {dto.Employee_Id} does not exist.");

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            // Determine cutoff date range: use the current Draft PayrollRun if available,
            // otherwise fall back to the current calendar month.
            var activeRun = await _context.PayrollRuns
                .Where(r => r.Status == "Draft" &&
                            r.CutOff_Start_Date <= today &&
                            r.CutOff_End_Date >= today)
                .FirstOrDefaultAsync();

            DateOnly cutoffStart;
            DateOnly cutoffEnd;

            if (activeRun != null)
            {
                cutoffStart = activeRun.CutOff_Start_Date;
                cutoffEnd = activeRun.CutOff_End_Date;
            }
            else
            {
                // Fallback: current calendar month
                cutoffStart = new DateOnly(today.Year, today.Month, 1);
                cutoffEnd = new DateOnly(today.Year, today.Month,
                    DateTime.DaysInMonth(today.Year, today.Month));
            }

            // Count days the employee has actually worked (Present or Late) in the cutoff period
            var daysWorked = await _context.AttendanceLogs
                .CountAsync(a =>
                    a.Employee_Id == dto.Employee_Id &&
                    a.Log_Date >= cutoffStart &&
                    a.Log_Date <= cutoffEnd &&
                    (a.Status == "Present" || a.Status == "Late"));

            var earnedValue = daysWorked * DefaultDailyRate;

            // Limit Gate: block requests that exceed what the employee has actually earned
            if (dto.Amount_Requested > earnedValue)
                throw new BusinessValidationException(
                    $"Cash advance request of {dto.Amount_Requested:C} exceeds earned value " +
                    $"of {earnedValue:C} ({daysWorked} day(s) worked × {DefaultDailyRate:C}/day).");

            var advance = new CashAdvance
            {
                Employee_Id = dto.Employee_Id,
                Amount_Requested = dto.Amount_Requested,
                Reason = dto.Reason,
                Status = "Pending"
            };

            _context.CashAdvances.Add(advance);
            await _context.SaveChangesAsync();

            return _mapper.Map<CashAdvanceReadDto>(advance);
        }

        public async Task<IEnumerable<CashAdvanceReadDto>> GetAllAsync()
        {
            var advances = await _context.CashAdvances
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<CashAdvanceReadDto>>(advances);
        }
    }
}
