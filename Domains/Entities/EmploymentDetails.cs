using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiHrm.Domains.Entities
{
    [Table("r_Employment_Details")]
    public class EmploymentDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmploymentDetailsId { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public DateOnly HireDate { get; set; }

        [StringLength(50)]
        public string? Tenure { get; set; } // Calculated field or manual entry

        [Required]
        [StringLength(50)]
        public string EmploymentStatus { get; set; } // Regular, Probationary, Training, Contractual

        [Required]
        [StringLength(100)]
        public string Department { get; set; }

        [Required]
        [StringLength(100)]
        public string Position { get; set; }

        [StringLength(20)]
        public string? SalaryGrade { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal BasePay { get; set; }

        [Column("Probationary_End_Date")]
        public DateOnly? ProbationaryEndDate { get; set; }

        // Navigation Property
        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }
    }
}
