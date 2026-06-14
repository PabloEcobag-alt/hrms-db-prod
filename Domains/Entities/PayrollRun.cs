using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiHrm.Domains.Entities
{
    [Table("r_Payroll_Run")]
    public class PayrollRun
    {
        [Key]
        [Column("payroll_run_id")]
        public int Id { get; set; }

        [Required]
        public DateOnly CutOff_Start_Date { get; set; }

        [Required]
        public DateOnly CutOff_End_Date { get; set; }

        public string Status { get; set; }

        public string? BatchReferenceNumber { get; set; }

        public string? Payout_Summary_Json { get; set; }

        public DateTime? Finalized_At { get; set; }

        public int? Finalized_By { get; set; }

        public virtual ICollection<EmployeePayrollRecord> EmployeePayrollRecords { get; set; }
        public virtual ICollection<BonusIncentive> BonusIncentives { get; set; }
        public virtual ICollection<Payslip> Payslips { get; set; }
        public virtual ICollection<PayoutSummary> PayoutSummaries { get; set; }
    }
}
