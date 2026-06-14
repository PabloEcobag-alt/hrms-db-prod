using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiHrm.Domains.Entities
{
    [Table("r_Employee_Payroll_Record")]
    public class EmployeePayrollRecord
    {
        [Key]
        [Column("payroll_record_id")]
        public int Id { get; set; }

        [Required]
        public int Payroll_Run_Id { get; set; }

        [Required]
        public int Employee_Id { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Basic_Pay { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal OT_Pay { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Sss_Deduction { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PhilHealth_Deduction { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PagIbig_Deduction { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Net_Pay { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Tax_Deduction { get; set; }

        public int Days_Worked { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal OT_Hours { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total_Deductions { get; set; }

        [ForeignKey("Payroll_Run_Id")]
        public virtual PayrollRun PayrollRun { get; set; }

        [ForeignKey("Employee_Id")]
        public virtual Employee Employee { get; set; }
    }
}
