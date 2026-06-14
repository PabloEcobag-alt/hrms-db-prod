using System.ComponentModel.DataAnnotations;

namespace Api.Contracts.Attendance
{
    public class AttendanceLogCreateDto
    {
        [Required]
        public int Employee_Id { get; set; }

        [Required]
        public DateOnly Log_Date { get; set; }

        [Required]
        public DateTime Time_In { get; set; }

        [Required]
        public string Log_Source { get; set; } = default!;
    }
}
