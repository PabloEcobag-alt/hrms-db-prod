using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ApiHrm.Infrastructures.Persistence;
using ApiHrm.Domains.Entities;
using Api.Contracts.Employee;
using Applications.Interfaces;
using Applications.Exceptions;

namespace Applications.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly hrmAppDbContext _context;
        private readonly IMapper _mapper;

        private static readonly HashSet<string> AllowedStatuses = new(StringComparer.Ordinal)
        {
            "Probationary", "Regular", "Resigned", "Terminated", "AWOL"
        };

        public EmployeeService(hrmAppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        private static void ValidateStatus(string? status)
        {
            if (status != null && !AllowedStatuses.Contains(status))
            {
                throw new BusinessValidationException(
                    "Status must be one of: Probationary, Regular, Resigned, Terminated, AWOL");
            }
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

        public async Task<EmployeeReadDto> CreateEmployeeAsync(EmployeeCreateDto createDto, string changedBy)
        {
            ValidateStatus(createDto.Status);

            var employee = _mapper.Map<Employee>(createDto);
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            
            await _context.Entry(employee).Reference(e => e.role).LoadAsync();

            _context.EmployeeHistories.Add(new EmployeeHistory
            {
                Employee_ID = employee.Employee_Id,
                Action_Type = "Employee Created",
                Old_Value = null,
                New_Value = $"Status: {employee.Status}, Role: {employee.role?.Role_Description}",
                Changed_By = changedBy,
                Changed_At = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            return _mapper.Map<EmployeeReadDto>(employee);
        }

        public async Task<bool> UpdateEmployeeAsync(int id, EmployeeUpdateDto updateDto, string changedBy)
        {
            var employee = await _context.Employees
                .Include(e => e.role)
                .FirstOrDefaultAsync(e => e.Employee_Id == id);
            if (employee == null) return false;

            ValidateStatus(updateDto.Status);

            var oldRoleId = employee.Role_ID;
            var oldRoleName = employee.role?.Role_Description;
            var oldStatus = employee.Status;

            _mapper.Map(updateDto, employee);

            if (!string.Equals(oldStatus, employee.Status, StringComparison.Ordinal))
            {
                _context.EmployeeHistories.Add(new EmployeeHistory
                {
                    Employee_ID = employee.Employee_Id,
                    Action_Type = "Status Change",
                    Old_Value = oldStatus,
                    New_Value = employee.Status,
                    Changed_By = changedBy,
                    Changed_At = DateTime.UtcNow
                });
            }

            if (oldRoleId != employee.Role_ID)
            {
                var newRoleName = await _context.Roles
                    .Where(r => r.Role_ID == employee.Role_ID)
                    .Select(r => r.Role_Description)
                    .FirstOrDefaultAsync();

                _context.EmployeeHistories.Add(new EmployeeHistory
                {
                    Employee_ID = employee.Employee_Id,
                    Action_Type = "Role Update",
                    Old_Value = oldRoleName,
                    New_Value = newRoleName,
                    Changed_By = changedBy,
                    Changed_At = DateTime.UtcNow
                });
            }

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