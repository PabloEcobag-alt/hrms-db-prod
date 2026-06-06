using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ApiHrm.Infrastructures.Persistence;
using ApiHrm.Domains.Entities;
using Api.Contracts.Employee;
using Applications.Interfaces;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Applications.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly hrmAppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmployeeService(hrmAppDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<EmployeeReadDto>> GetAllEmployeesAsync()
        {
            var employees = await _context.Employees.Include(e => e.role).ToListAsync();
            return _mapper.Map<IEnumerable<EmployeeReadDto>>(employees);
        }

        public async Task<EmployeeDetailDto?> GetEmployeeByIdAsync(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.role)
                .Include(e => e.Documents)
                .Include(e => e.Exits)
                .FirstOrDefaultAsync(e => e.Employee_Id == id);

            return _mapper.Map<EmployeeDetailDto>(employee);
        }

        public async Task<EmployeeReadDto> CreateEmployeeAsync(EmployeeCreateDto createDto)
        {
            var employee = _mapper.Map<Employee>(createDto);
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            
            await _context.Entry(employee).Reference(e => e.role).LoadAsync();
            return _mapper.Map<EmployeeReadDto>(employee);
        }

        public async Task<bool> UpdateEmployeeAsync(int id, EmployeeUpdateDto updateDto)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return false;

            // Capture old state for audit trail (limited to 255 chars)
            var oldStateJson = JsonSerializer.Serialize(employee);
            var truncatedOldState = oldStateJson.Length > 255 ? oldStateJson.Substring(0, 255) : oldStateJson;

            _mapper.Map(updateDto, employee);

            // Capture new state for audit trail (limited to 255 chars)
            var newStateJson = JsonSerializer.Serialize(employee);
            var truncatedNewState = newStateJson.Length > 255 ? newStateJson.Substring(0, 255) : newStateJson;

            // Get current user from JWT claims
            var user = _httpContextAccessor.HttpContext?.User;
            var changedBy = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                ?? user?.FindFirst("sub")?.Value 
                ?? user?.FindFirst(ClaimTypes.Email)?.Value 
                ?? "System";

            // Create audit history record
            var history = new EmployeeHistory
            {
                Employee_ID = employee.Employee_Id,
                Action_Type = "Update",
                Old_Value = truncatedOldState,
                New_Value = truncatedNewState,
                Changed_By = changedBy,
                Changed_At = DateTime.UtcNow
            };

            _context.EmployeeHistories.Add(history);
            
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return false;

            _context.Employees.Remove(employee);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}