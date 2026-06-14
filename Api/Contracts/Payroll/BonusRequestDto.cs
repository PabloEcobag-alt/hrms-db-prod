using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Contracts.Payroll
{
    public class BonusRequestDto
    {
        [Required]
        public int Payroll_Run_Id { get; set; }

        [Required]
        public int Employee_Id { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Bonus_Amount { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Incentive_Amount { get; set; }
    }
}
