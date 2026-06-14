namespace Api.Contracts.Leave
{
    public class LeaveRequestReadDto
    {
        public int Id { get; set; }
        public int Employee_Id { get; set; }
        public int LeaveTypeId { get; set; }
        public string LeaveTypeLabel { get; set; } = default!;
        public DateOnly Start_Date { get; set; }
        public DateOnly End_Date { get; set; }
        public string Reason { get; set; } = default!;
        public string Status { get; set; } = default!;
    }
}
