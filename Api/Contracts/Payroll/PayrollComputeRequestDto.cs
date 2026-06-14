using System.ComponentModel.DataAnnotations;

namespace Api.Contracts.Payroll
{
    public class PayrollComputeRequestDto
    {
        [Required]
        public int Employee_Id { get; set; }

        [Required]
        public DateOnly Cutoff_Date { get; set; }
    }
}
