using System.ComponentModel.DataAnnotations;

namespace Api.Contracts.CashAdvance
{
    public class CashAdvanceCreateDto
    {
        [Required]
        public int Employee_Id { get; set; }

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Amount requested must be greater than zero.")]
        public decimal Amount_Requested { get; set; }

        public string Reason { get; set; } = default!;
    }
}
