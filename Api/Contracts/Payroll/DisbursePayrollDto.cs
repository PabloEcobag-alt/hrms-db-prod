using System.ComponentModel.DataAnnotations;

namespace Api.Contracts.Payroll
{
    public class DisbursePayrollDto
    {
        [Required]
        public string BatchReferenceNumber { get; set; } = default!;
    }
}
