using Api.Contracts.Attendance;

namespace Applications.Interfaces
{
    public interface IAttendanceService
    {
        Task<AttendanceLogReadDto> LogAsync(AttendanceLogCreateDto dto);
        Task<AttendanceLogReadDto> OverrideAsync(AttendanceOverrideDto dto, string userId, string ipAddress);
    }
}
