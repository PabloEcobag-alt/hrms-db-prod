using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiHrm.Domains.Entities
{
    [Table("r_Email_Dispatch_Log")]
    public class EmailDispatchLog
    {
        [Key]
        [Column("email_dispatch_log_id")]
        public int Id { get; set; }

        [Required]
        public int PayslipId { get; set; }

        [Required]
        [MaxLength(255)]
        public string EmployeeEmail { get; set; }

        public DateTime? SentAt { get; set; }

        public int RetryCount { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; }

        [MaxLength(500)]
        public string? ErrorMessage { get; set; }

        [ForeignKey("PayslipId")]
        public virtual Payslip Payslip { get; set; }
    }
}
