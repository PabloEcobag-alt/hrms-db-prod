using Api.Contracts.Checklist;

namespace Applications.Interfaces
{
    public interface IChecklistService
    {
        Task<ChecklistReadDto?> GetChecklistByApplicantIdAsync(int applicantId);
        Task<bool> UpdateChecklistAsync(ChecklistUpdateDto updateDto);
    }
}