using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ApiHrm.Domains.Entities;
using ApiHrm.Infrastructures.Persistence;
using Api.Contracts.Payroll;
using Applications.Exceptions;
using Applications.Interfaces;

namespace Applications.Services
{
    public class PayrollService : IPayrollService
    {
        // Placeholder daily rate used until a contract rate field is added to Employee.
        private const decimal DefaultDailyRate = 500m;

        // Standard working hours per day used for OT hourly rate computation.
        private const decimal StandardHoursPerDay = 8m;

        // OT premium multiplier (DOLE standard: 25% above regular hourly rate).
        private const decimal OtMultiplier = 1.25m;

        // Mandatory contribution rates — 2024 schedule placeholders.
        // TODO: Replace with DB-driven bracket tables when contribution seeding is implemented.
        private const decimal SssRate       = 0.045m;
        private const decimal PhilHealthRate = 0.025m;
        private const decimal PagIbigFixed  = 100m;

        private readonly hrmAppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAuditLogService _auditLogService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PayrollService(
            hrmAppDbContext context,
            IMapper mapper,
            IAuditLogService auditLogService,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _auditLogService = auditLogService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PayrollComputeResultDto> ComputeAsync(int employeeId, DateOnly cutoffDate)
        {
            var employeeExists = await _context.Employees.AnyAsync(e => e.Employee_Id == employeeId);
            if (!employeeExists)
                throw new BusinessValidationException($"Employee with ID {employeeId} does not exist.");

            // Resolve cutoff period: use active Draft PayrollRun covering cutoffDate,
            // falling back to the calendar month of the supplied date.
            var activeRun = await _context.PayrollRuns
                .Where(r => r.Status == "Draft" &&
                            r.CutOff_Start_Date <= cutoffDate &&
                            r.CutOff_End_Date >= cutoffDate)
                .FirstOrDefaultAsync();

            DateOnly periodStart;
            DateOnly periodEnd;

            if (activeRun != null)
            {
                periodStart = activeRun.CutOff_Start_Date;
                periodEnd   = activeRun.CutOff_End_Date;
            }
            else
            {
                periodStart = new DateOnly(cutoffDate.Year, cutoffDate.Month, 1);
                periodEnd   = new DateOnly(cutoffDate.Year, cutoffDate.Month,
                    DateTime.DaysInMonth(cutoffDate.Year, cutoffDate.Month));
            }

            // Count worked days (Present or Late) within the cutoff period.
            var daysWorked = await _context.AttendanceLogs
                .CountAsync(a =>
                    a.Employee_Id == employeeId &&
                    a.Log_Date >= periodStart &&
                    a.Log_Date <= periodEnd &&
                    (a.Status == "Present" || a.Status == "Late"));

            // Sum OT hours: any day where hours worked exceed StandardHoursPerDay.
            var logsWithTimeOut = await _context.AttendanceLogs
                .Where(a =>
                    a.Employee_Id == employeeId &&
                    a.Log_Date >= periodStart &&
                    a.Log_Date <= periodEnd &&
                    a.Time_In != null &&
                    a.Time_Out != null)
                .ToListAsync();

            var otHours = logsWithTimeOut
                .Sum(a =>
                {
                    var hoursWorked = (decimal)(a.Time_Out!.Value - a.Time_In!.Value).TotalHours;
                    return Math.Max(0m, hoursWorked - StandardHoursPerDay);
                });

            // Compute earnings.
            var basicPay  = daysWorked * DefaultDailyRate;
            var hourlyRate = DefaultDailyRate / StandardHoursPerDay;
            var otPay      = otHours * hourlyRate * OtMultiplier;

            // Compute mandatory deductions.
            var sssDeduction        = basicPay * SssRate;
            var philHealthDeduction = basicPay * PhilHealthRate;
            var pagIbigDeduction    = PagIbigFixed;
            var totalDeductions     = sssDeduction + philHealthDeduction + pagIbigDeduction;

            var netPay = basicPay + otPay - totalDeductions;

            // Build itemised line-items for the breakdown DTO.
            var lineItems = new List<PayrollLineItemDto>
            {
                new() { Name = "Basic Pay",              Amount = basicPay,           Type = "Earnings"  },
                new() { Name = "Overtime Pay",           Amount = otPay,              Type = "Earnings"  },
                new() { Name = "SSS Deduction",          Amount = sssDeduction,       Type = "Deduction" },
                new() { Name = "PhilHealth Deduction",   Amount = philHealthDeduction, Type = "Deduction" },
                new() { Name = "Pag-IBIG Deduction",     Amount = pagIbigDeduction,   Type = "Deduction" }
            };

            return new PayrollComputeResultDto
            {
                Employee_Id         = employeeId,
                Cutoff_Date         = cutoffDate,
                Payroll_Run_Id      = activeRun?.Id,
                Daily_Rate          = DefaultDailyRate,
                Days_Worked         = daysWorked,
                Basic_Pay           = basicPay,
                OT_Hours            = otHours,
                OT_Pay              = otPay,
                Sss_Deduction       = sssDeduction,
                PhilHealth_Deduction = philHealthDeduction,
                PagIbig_Deduction   = pagIbigDeduction,
                Total_Deductions    = totalDeductions,
                Net_Pay             = netPay,
                Line_Items          = lineItems
            };
        }

        public async Task<PayrollRecordReadDto> FinalizeAsync(PayrollFinalizeRequestDto dto)
        {
            if (dto.Payroll_Run_Id == 0)
                throw new BusinessValidationException(
                    "Payroll_Run_Id is required. Run /api/payroll/compute first to obtain a valid Payroll_Run_Id.");

            var run = await _context.PayrollRuns.FindAsync(dto.Payroll_Run_Id);
            if (run == null)
                throw new BusinessValidationException($"Payroll run with ID {dto.Payroll_Run_Id} does not exist.");

            if (run.Status == "Finalized")
                throw new BusinessValidationException(
                    $"Payroll run {dto.Payroll_Run_Id} is already finalized and cannot be modified.");

            var employeeExists = await _context.Employees.AnyAsync(e => e.Employee_Id == dto.Employee_Id);
            if (!employeeExists)
                throw new BusinessValidationException($"Employee with ID {dto.Employee_Id} does not exist.");

            // Upsert: update existing record for this employee + run, or create a new one.
            var record = await _context.EmployeePayrollRecords
                .FirstOrDefaultAsync(r =>
                    r.Payroll_Run_Id == dto.Payroll_Run_Id &&
                    r.Employee_Id == dto.Employee_Id);

            if (record == null)
            {
                record = new EmployeePayrollRecord
                {
                    Payroll_Run_Id = dto.Payroll_Run_Id,
                    Employee_Id    = dto.Employee_Id
                };
                _context.EmployeePayrollRecords.Add(record);
            }

            record.Basic_Pay           = dto.Basic_Pay;
            record.OT_Pay              = dto.OT_Pay;
            record.Sss_Deduction       = dto.Sss_Deduction;
            record.PhilHealth_Deduction = dto.PhilHealth_Deduction;
            record.PagIbig_Deduction   = dto.PagIbig_Deduction;
            record.Net_Pay             = dto.Net_Pay;

            // Lock the payroll run.
            run.Status = "Finalized";

            await _context.SaveChangesAsync();

            // Audit trail.
            var httpContext = _httpContextAccessor.HttpContext;
            var userId   = httpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "unknown";
            var userRole = httpContext?.User.FindFirstValue(ClaimTypes.Role)           ?? "unknown";
            var ipAddress = httpContext?.Connection.RemoteIpAddress?.ToString()        ?? "Unknown";

            await _auditLogService.LogAsync(
                userId: userId,
                userRole: userRole,
                action: "FINALIZE",
                module: "Payroll",
                recordId: record.Id.ToString(),
                recordLabel: $"Payroll record for Employee {dto.Employee_Id} in Run {dto.Payroll_Run_Id}",
                ipAddress: ipAddress);

            // TODO: Implement using QuestPDF for PDF payslip generation.
            // Generate a formatted payslip PDF, store it in cloud/local storage,
            // and save a reference in the r_Payslip table keyed to employee_id + payroll_run_id.

            // TODO: Configure SMTP for Payslip Email.
            // After PDF generation, dispatch the payslip as an email attachment to the
            // employee's registered email address. Log failed deliveries with a retry flag.

            return _mapper.Map<PayrollRecordReadDto>(record);
        }

        public async Task<bool> DisbursePayrollAsync(int payrollId, DisbursePayrollDto dto, string disburserUserId, string disburserRole, string ipAddress)
        {
            var run = await _context.PayrollRuns.FindAsync(payrollId);
            if (run == null) return false;

            if (run.Status != "Finalized")
                throw new BusinessValidationException("Only finalized payrolls can be disbursed.");

            run.Status = "Disbursed";
            run.BatchReferenceNumber = dto.BatchReferenceNumber;

            await _context.SaveChangesAsync();

            // AuditLog captures: "Payroll [Id] disbursed by HR Admin [UserId] with Batch Reference [BatchRef]"
            var auditMsg = $"Payroll {payrollId} disbursed by HR Admin {disburserUserId} with Batch Reference {dto.BatchReferenceNumber}";

            await _auditLogService.LogAsync(
                userId: disburserUserId,
                userRole: disburserRole,
                action: "DISBURSE",
                module: "Payroll",
                recordId: payrollId.ToString(),
                recordLabel: auditMsg,
                ipAddress: ipAddress
            );

            return true;
        }

        public async Task<IEnumerable<PayrollRunListItemDto>> GetPayrollRunsAsync()
        {
            var records = await _context.EmployeePayrollRecords
                .Include(r => r.Employee)
                .Include(r => r.PayrollRun)
                .ToListAsync();

            var dtos = new List<PayrollRunListItemDto>();
            foreach (var record in records)
            {
                var dto = new PayrollRunListItemDto
                {
                    Id = record.Id,
                    Employee_Id = record.Employee_Id,
                    Employee_Name = $"{record.Employee.First_Name} {record.Employee.Last_Name}",
                    Position = record.Employee.Position,
                    Basic_Pay = record.Basic_Pay,
                    OT_Pay = record.OT_Pay,
                    Sss_Deduction = record.Sss_Deduction,
                    PhilHealth_Deduction = record.PhilHealth_Deduction,
                    PagIbig_Deduction = record.PagIbig_Deduction,
                    Tax = 0,
                    Bonus = 0,
                    Net_Pay = record.Net_Pay,
                    Status = record.PayrollRun?.Status ?? "Unknown",
                    Payout_Method = record.Employee.PaymentMethod
                };
                dtos.Add(dto);
            }

            return dtos;
        }
    }
}
