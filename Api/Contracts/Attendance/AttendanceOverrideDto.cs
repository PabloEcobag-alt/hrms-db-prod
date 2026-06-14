using System.ComponentModel.DataAnnotations;

namespace Api.Contracts.Attendance
{
    public class AttendanceOverrideDto
    {
        [Required]
        public int Employee_Id { get; set; }

        [Required]
        public DateOnly Date { get; set; }

        [Required]
        public DateTime Corrected_Time_In { get; set; }

        [Required]
        public DateTime Corrected_Time_Out { get; set; }

        [Required]
        public string Reason { get; set; } = default!;
    }
}
