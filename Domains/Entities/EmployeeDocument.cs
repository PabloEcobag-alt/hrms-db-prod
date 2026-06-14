using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiHrm.Domains.Entities
{
    [Table("r_Employee_Document")]
    public class EmployeeDocument
    {
        [Key]
        [Column("employee_document_id")]
        public int Id { get; set; }

        [Required]
        public int Employee_Id { get; set; }

        public string Document_Type { get; set; }

        [Required]
        public DateOnly Issue_Date { get; set; }

        [Required]
        public DateOnly Expiry_Date { get; set; }

        public string File_Url { get; set; }

        public string Status { get; set; }

        public string VerificationStatus { get; set; } = "Pending_Verification";

        public int? VerifiedByUserId { get; set; }

        public string? RejectionReason { get; set; }

        [ForeignKey("Employee_Id")]
        public virtual Employee Employee { get; set; }
    }
}
