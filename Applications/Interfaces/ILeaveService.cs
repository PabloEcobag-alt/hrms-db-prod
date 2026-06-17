using Api.Contracts.Leave;

namespace Applications.Interfaces
{
    public interface ILeaveService
    {
        Task<LeaveRequestReadDto> CreateAsync(LeaveRequestCreateDto dto);
        Task<LeaveRequestReadDto> UpdateStatusAsync(int id, LeaveStatusUpdateDto dto);
        Task<IEnumerable<LeaveRequestReadDto>> GetAllAsync();
    }
}
