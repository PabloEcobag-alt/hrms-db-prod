using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiHrm.Domains.Entities
{
    [Table("r_Employee_Document")]
    public class EmployeeDocument
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DocumentId { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        [StringLength(255)]
        public string DocumentName { get; set; }

        [Required]
        [StringLength(100)]
        public string DocumentType { get; set; } // Contract, ID, Certificate, etc.

        [Required]
        [StringLength(1000)]
        public string FileUrl { get; set; }

        [Required]
        public DateTime UploadDate { get; set; } = DateTime.UtcNow;

        // Additional existing fields (retained for compatibility)
        public string? Status { get; set; }
        public string VerificationStatus { get; set; } = "Pending_Verification";
        public int? VerifiedByUserId { get; set; }
        public string? RejectionReason { get; set; }

        // Legacy fields maintained for compatibility
        public DateOnly Issue_Date { get; set; }
        public DateOnly? Expiry_Date { get; set; }

        // Navigation Property
        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }
    }
}
