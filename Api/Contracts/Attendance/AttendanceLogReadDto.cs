namespace Api.Contracts.Attendance
{
    public class AttendanceLogReadDto
    {
        public int Id { get; set; }
        public int Employee_Id { get; set; }
        public DateOnly Log_Date { get; set; }
        public DateTime? Time_In { get; set; }
        public DateTime? Time_Out { get; set; }
        public string Status { get; set; } = default!;
        public string Log_Source { get; set; } = default!;
        public bool Is_Manual_Override { get; set; }
    }
}
