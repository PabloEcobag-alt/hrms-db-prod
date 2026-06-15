using Api.Contracts.Employee;

namespace Applications.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync();
        Task<EmployeeDto?> GetEmployeeByIdAsync(int id);
        Task<EmployeeDto> CreateEmployeeAsync(EmployeeCreateDto createDto);
        Task<bool> UpdateEmployeeProfileAsync(int id, EmployeeUpdateDto updateDto);
        Task<bool> DeleteEmployeeAsync(int id);
    }
}