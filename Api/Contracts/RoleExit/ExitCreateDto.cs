using System.ComponentModel.DataAnnotations;

namespace Api.Contracts.RoleExit
{
    public class ExitCreateDto
    {
        [Required]
        public int Employee_ID { get; set; }

        [Required]
        public DateTime Date_Of_Exit { get; set; }

        [Required]
        public string Reason_For_Leaving { get; set; } = default!;

        public string? Interview_Notes { get; set; }

        [Required]
        public string Interviewed_By { get; set; } = default!;
    }
}