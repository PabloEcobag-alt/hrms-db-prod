using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiHrm.Domains.Entities
{
    [Table("r_Document_Status")]
    public class DocumentStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int Employee_Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Document_Type { get; set; } // Expected: "personal" | "government" | "company" | "performance"

        [Required]
        public bool Is_Completed { get; set; }

        public DateOnly Last_Updated { get; set; }

        public DateOnly? Expiry_Date { get; set; }

        [ForeignKey("Employee_Id")]
        public virtual Employee Employee { get; set; }
    }
}
