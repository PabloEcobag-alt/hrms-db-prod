using Api.Contracts.Digital201;

namespace Applications.Interfaces
{
    public interface IDigital201Service
    {
        Task<int> CreateEmployeeAsync(CreateEmployeeDto dto);
        Task<List<AdminEmployeeListDto>> GetAllEmployeesAsync();
        Task<GetContactInfoResponseDto?> GetContactInfoAsync(string erpUserId);
        Task<bool> UpdateContactInfoAsync(string erpUserId, UpdateContactInfoRequestDto request);
    }
}
