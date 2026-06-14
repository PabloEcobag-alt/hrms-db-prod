using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiHrm.Domains.Entities
{
    [Table("r_Payslip")]
    public class Payslip
    {
        [Key]
        [Column("payslip_id")]
        public int Id { get; set; }

        [Required]
        public int Payroll_Run_Id { get; set; }

        [Required]
        public int Employee_Id { get; set; }

        [Required]
        public DateOnly Payout_Date { get; set; }

        public string Pdf_Url { get; set; }

        public string Download_Url { get; set; }

        public DateTime? Email_Sent_At { get; set; }

        public int Email_Retry_Count { get; set; }

        [ForeignKey("Payroll_Run_Id")]
        public virtual PayrollRun PayrollRun { get; set; }

        [ForeignKey("Employee_Id")]
        public virtual Employee Employee { get; set; }
    }
}
