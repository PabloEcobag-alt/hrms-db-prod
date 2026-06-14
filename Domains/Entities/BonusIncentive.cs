using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiHrm.Domains.Entities
{
    [Table("r_Bonus_Incentive")]
    public class BonusIncentive
    {
        [Key]
        [Column("bonus_incentive_id")]
        public int Id { get; set; }

        [Required]
        public int Payroll_Run_Id { get; set; }

        [Required]
        public int Employee_Id { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Bonus_Amount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Incentive_Amount { get; set; }

        [ForeignKey("Payroll_Run_Id")]
        public virtual PayrollRun PayrollRun { get; set; }

        [ForeignKey("Employee_Id")]
        public virtual Employee Employee { get; set; }
    }
}
