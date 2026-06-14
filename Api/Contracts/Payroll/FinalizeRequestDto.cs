using System.ComponentModel.DataAnnotations;

namespace Api.Contracts.Payroll
{
    public class FinalizeRequestDto
    {
        [Required]
        public int Payroll_Run_Id { get; set; }
    }
}
