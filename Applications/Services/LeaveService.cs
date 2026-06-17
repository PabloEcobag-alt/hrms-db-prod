using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ApiHrm.Domains.Entities;
using ApiHrm.Infrastructures.Persistence;
using Api.Contracts.Leave;
using Applications.Exceptions;
using Applications.Interfaces;

namespace Applications.Services
{
    public class LeaveService : ILeaveService
    {
        private readonly hrmAppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAuditLogService _auditLogService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LeaveService(
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

        public async Task<LeaveRequestReadDto> CreateAsync(LeaveRequestCreateDto dto)
        {
            var employeeExists = await _context.Employees.AnyAsync(e => e.EmployeeId == dto.Employee_Id);
            if (!employeeExists)
                throw new BusinessValidationException($"Employee with ID {dto.Employee_Id} does not exist.");

            // 7-Day Advance Notice Rule: reject submissions with less than 7 days lead time
            var today = DateTime.UtcNow.Date;
            var startDateTime = dto.Start_Date.ToDateTime(TimeOnly.MinValue);
            var daysUntilLeave = (startDateTime - today).Days;

            if (daysUntilLeave < 7)
                throw new BusinessValidationException(
                    $"Leave must be submitted at least 7 days in advance. " +
                    $"Your selected start date is only {daysUntilLeave} day(s) away.");

            var leaveType = await _context.LeaveTypes.FindAsync(dto.LeaveTypeId);
            if (leaveType == null)
                throw new BusinessValidationException($"Leave type ID {dto.LeaveTypeId} does not exist.");

            var request = new LeaveRequest
            {
                Employee_Id = dto.Employee_Id,
                LeaveTypeId = dto.LeaveTypeId,
                Start_Date = dto.Start_Date,
                End_Date = dto.End_Date,
                Reason = dto.Reason,
                Status = "Pending"
            };

            _context.LeaveRequests.Add(request);
            await _context.SaveChangesAsync();

            var httpContext = _httpContextAccessor.HttpContext;
            var userId   = httpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "unknown";
            var userRole = httpContext?.User.FindFirstValue(ClaimTypes.Role)           ?? "unknown";
            var ipAddress = httpContext?.Connection.RemoteIpAddress?.ToString()        ?? "Unknown";

            await _auditLogService.LogAsync(
                userId: userId,
                userRole: userRole,
                action: "CREATE",
                module: "Leave",
                recordId: request.Id.ToString(),
                recordLabel: "Leave Request",
                ipAddress: ipAddress);

            // Reload with navigation property for mapping
            await _context.Entry(request).Reference(r => r.LeaveType).LoadAsync();

            return _mapper.Map<LeaveRequestReadDto>(request);
        }

        public async Task<LeaveRequestReadDto> UpdateStatusAsync(int id, LeaveStatusUpdateDto dto)
        {
            var request = await _context.LeaveRequests
                .Include(r => r.LeaveType)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request == null)
                throw new BusinessValidationException($"Leave request with ID {id} was not found.");

            request.Status = dto.Status;

            if (dto.Status == "Declined")
            {
                // TODO: Refund Leave Credits — when a request is declined, immediately
                // release and refund the locked vacation or sick balance allocations
                // back to the employee's LeaveBalance record (r_Leave_Balance).
                // Query: find the LeaveBalance row for this employee + leave code + current year,
                // then subtract the pending days and restore the used/allocated balance.
            }

            await _context.SaveChangesAsync();

            return _mapper.Map<LeaveRequestReadDto>(request);
        }
    }
}
