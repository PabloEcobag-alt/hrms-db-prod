using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ApiHrm.Infrastructures.Persistence;
using ApiHrm.Domains.Entities;
using Api.Contracts.Employee;
using Applications.Interfaces;

namespace Applications.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly hrmAppDbContext _context;
        private readonly IMapper _mapper;

        public EmployeeService(hrmAppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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

            _mapper.Map(updateDto, employee);
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