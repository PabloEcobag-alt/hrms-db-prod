using Api.Contracts.RoleExit;

namespace Applications.Interfaces
{
    public interface IExitService
    {
        Task<IEnumerable<ExitReadDto>> GetAllExitsAsync();
        
        Task<ExitReadDto?> GetExitByIdAsync(int id);
        
        Task<ExitReadDto> CreateExitRecordAsync(ExitCreateDto exitDto);
        
        Task<bool> UpdateExitRecordAsync(int id, ExitCreateDto updateDto);
    }
}