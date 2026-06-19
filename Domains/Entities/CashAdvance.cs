using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiHrm.Domains.Entities
{
    [Table("r_Cash_Advance")]
    public class CashAdvance
    {
        [Key]
        [Column("cash_advance_id")]
        public int Id { get; set; }

        [Required]
        public int Employee_Id { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount_Requested { get; set; }

        public string Reason { get; set; }

        public string Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("Employee_Id")]
        public virtual Employee Employee { get; set; }
    }
}
