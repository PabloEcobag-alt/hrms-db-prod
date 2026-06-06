using Api.Contracts.Employee;

namespace Applications.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeReadDto>> GetAllEmployeesAsync();
        Task<EmployeeDetailDto?> GetEmployeeByIdAsync(int id);
        Task<EmployeeReadDto> CreateEmployeeAsync(EmployeeCreateDto createDto, string changedBy);
        Task<bool> UpdateEmployeeAsync(int id, EmployeeUpdateDto updateDto, string changedBy);
        Task<bool> DeleteEmployeeAsync(int id);
    }
}