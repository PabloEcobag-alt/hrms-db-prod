using System.ComponentModel.DataAnnotations;

namespace Api.Contracts.Leave
{
    public class LeaveStatusUpdateDto
    {
        [Required]
        public string Status { get; set; } = default!;
    }
}
