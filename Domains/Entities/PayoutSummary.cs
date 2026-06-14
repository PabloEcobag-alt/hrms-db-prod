using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiHrm.Domains.Entities
{
    [Table("r_Payout_Summary")]
    public class PayoutSummary
    {
        [Key]
        [Column("payout_summary_id")]
        public int Id { get; set; }

        [Required]
        public int PayrollRunId { get; set; }

        [Required]
        [MaxLength(50)]
        public string PaymentMethod { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public int EmployeeCount { get; set; }

        [ForeignKey("PayrollRunId")]
        public virtual PayrollRun PayrollRun { get; set; }
    }
}
