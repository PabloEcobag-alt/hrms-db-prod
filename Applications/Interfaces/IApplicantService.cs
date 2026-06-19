using Api.Contracts.Applicant;
using Api.Contracts.Employee;
using Api.Contracts.RoleExit;

namespace Applications.Interfaces
{
    public interface IApplicantService
    {
        Task<IEnumerable<ApplicantReadDto>> GetAllApplicantsAsync();
        Task<ApplicantDetailDto?> GetApplicantByIdAsync(int id);
        Task<ApplicantReadDto> CreateApplicantAsync(ApplicantCreateDto createDto);
        Task<bool> UpdateApplicantStatusAsync(int id, string status);
        Task<ApplicantReadDto?> UpdateApplicantAsync(int id, ApplicantUpdateDto updateDto);
        Task<EmployeeReadDto?> HireApplicantAsync(int applicantId, int roleId);
        Task<ApplicantReadDto> CreateEcommerceApplicationAsync(EcommerceApplicationDto dto);
        Task<ApplicantReadDto?> GetRecentEcommerceApplicantAsync();
    }
}