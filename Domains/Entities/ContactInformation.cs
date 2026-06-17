using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiHrm.Domains.Entities
{
    [Table("r_Contact_Information")]
    public class ContactInformation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ContactInformationId { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string EmailAddress { get; set; }

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [StringLength(20)]
        public string? TelephoneNumber { get; set; }

        [StringLength(500)]
        public string? PresentAddress { get; set; }

        [StringLength(500)]
        public string? PermanentAddress { get; set; }

        // Navigation Property
        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }
    }
}
