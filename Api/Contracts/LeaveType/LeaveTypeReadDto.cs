namespace Api.Contracts.LeaveType
{
    public class LeaveTypeReadDto
    {
        public int LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; } = default!;
        public string? Description { get; set; }
    }
}
