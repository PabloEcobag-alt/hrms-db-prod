using System.ComponentModel.DataAnnotations;

namespace Api.Contracts.Leave
{
    public class LeaveRequestCreateDto
    {
        [Required]
        public int Employee_Id { get; set; }

        [Required]
        public int LeaveTypeId { get; set; }

        [Required]
        public DateOnly Start_Date { get; set; }

        [Required]
        public DateOnly End_Date { get; set; }

        public string Reason { get; set; } = default!;
    }
}
