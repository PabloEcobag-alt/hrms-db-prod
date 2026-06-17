using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ApiHrm.Domains.Entities;
using ApiHrm.Infrastructures.Persistence;
using Api.Contracts.Attendance;
using Applications.Exceptions;
using Applications.Interfaces;

namespace Applications.Services
{
    public class AttendanceService : IAttendanceService
    {
        // Shift start times mapped from shift code constants
        private static readonly Dictionary<string, TimeSpan> ShiftStartTimes = new()
        {
            { "SHIFT_6_3",  new TimeSpan(6,  0, 0) },
            { "SHIFT_9_6",  new TimeSpan(9,  0, 0) },
            { "SHIFT_11_8", new TimeSpan(11, 0, 0) }
        };

        private readonly hrmAppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAuditLogService _auditLogService;

        public AttendanceService(
            hrmAppDbContext context,
            IMapper mapper,
            IAuditLogService auditLogService)
        {
            _context = context;
            _mapper = mapper;
            _auditLogService = auditLogService;
        }

        public async Task<AttendanceLogReadDto> LogAsync(AttendanceLogCreateDto dto)
        {
            var employeeExists = await _context.Employees.AnyAsync(e => e.EmployeeId == dto.Employee_Id);
            if (!employeeExists)
                throw new BusinessValidationException($"Employee with ID {dto.Employee_Id} does not exist.");

            // Sunday Drop Rule: hard-reject any biometric log dated on a Sunday
            if (dto.Log_Date.DayOfWeek == DayOfWeek.Sunday)
                throw new BusinessValidationException("Attendance logs are not accepted on Sundays.");

            // Shift Window Check: determine Late or Present based on active shift
            var shift = await _context.EmployeeShifts
                .Where(s =>
                    s.Employee_Id == dto.Employee_Id &&
                    s.Effective_Date <= dto.Log_Date &&
                    (s.End_Date == null || s.End_Date >= dto.Log_Date))
                .OrderByDescending(s => s.Effective_Date)
                .FirstOrDefaultAsync();

            var status = "Present";

            if (shift != null && ShiftStartTimes.TryGetValue(shift.Shift_Code, out var shiftStart))
            {
                status = dto.Time_In.TimeOfDay > shiftStart ? "Late" : "Present";
            }

            var log = new AttendanceLog
            {
                Employee_Id = dto.Employee_Id,
                Log_Date = dto.Log_Date,
                Time_In = dto.Time_In,
                Status = status,
                Log_Source = dto.Log_Source,
                Is_Manual_Override = false
            };

            _context.AttendanceLogs.Add(log);
            await _context.SaveChangesAsync();

            return _mapper.Map<AttendanceLogReadDto>(log);
        }

        public async Task<AttendanceLogReadDto> OverrideAsync(
            AttendanceOverrideDto dto,
            string userId,
            string ipAddress)
        {
            var employeeExists = await _context.Employees.AnyAsync(e => e.EmployeeId == dto.Employee_Id);
            if (!employeeExists)
                throw new BusinessValidationException($"Employee with ID {dto.Employee_Id} does not exist.");

            // Write audit trail BEFORE applying the override
            await _auditLogService.LogAsync(
                userId: userId,
                userRole: "HR",
                action: "UPDATE",
                module: "Attendance",
                recordId: dto.Employee_Id.ToString(),
                recordLabel: $"Manual override for Employee {dto.Employee_Id} on {dto.Date}",
                ipAddress: ipAddress);

            var log = new AttendanceLog
            {
                Employee_Id = dto.Employee_Id,
                Log_Date = dto.Date,
                Time_In = dto.Corrected_Time_In,
                Time_Out = dto.Corrected_Time_Out,
                Status = "Present",
                Log_Source = "Manual",
                Is_Manual_Override = true
            };

            _context.AttendanceLogs.Add(log);
            await _context.SaveChangesAsync();

            return _mapper.Map<AttendanceLogReadDto>(log);
        }
    }
}
