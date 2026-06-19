using Api.Contracts.Digital201;

namespace Applications.Interfaces
{
    public interface IDigital201Service
    {
        Task<int> CreateEmployeeAsync(CreateEmployeeDto dto);
        Task<List<AdminEmployeeListDto>> GetAllEmployeesAsync();
        Task<GetContactInfoResponseDto?> GetContactInfoAsync(string erpUserId);
        Task<bool> UpdateContactInfoAsync(string erpUserId, UpdateContactInfoRequestDto request);
        Task<EmployeeProfileDto?> GetEmployeeProfileAsync(int employeeId);
        Task<EmployeeProfileDto?> GetEmployeeByErpUserIdAsync(string erpUserId);
        Task<bool> UpdateEmployeeAsync(int employeeId, UpdateEmployeeDto dto);
        Task<List<UnregisteredEmployeeDto>> GetUnregisteredEmployeesAsync();
        Task<bool> UpdateEmployeeErpUserIdAsync(int employeeId, string erpUserId);
        Task<List<ExpiringDocumentDto>> GetExpiringDocumentsAsync(int days);
    }
}
