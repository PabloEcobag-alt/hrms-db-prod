using MediatR;

namespace Applications.Events
{
    public record ApplicantHiredEvent : INotification
    {
        public int ApplicantId { get; init; }
        public int EmployeeId { get; init; }
        public DateTime HiredAt { get; init; }
        public string Email { get; init; } = string.Empty;
        public string Phone { get; init; } = string.Empty;
        public string Position { get; init; } = string.Empty;
    }
}