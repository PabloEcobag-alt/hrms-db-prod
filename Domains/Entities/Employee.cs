using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiHrm.Domains.Entities
{
    [Table("r_Employee_Record")]

    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmployeeId { get; set; }

        [StringLength(100)]
        public string? ErpUserId { get; set; } // FK to ms-authentication service

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [StringLength(100)]
        public string? MiddleName { get; set; }

        [Required]
        public DateOnly DateOfBirth { get; set; }

        [Required]
        [StringLength(20)]
        public string Gender { get; set; } // Male, Female, Other

        [Required]
        [StringLength(20)]
        public string CivilStatus { get; set; } // Single, Married, Divorced, Widowed

        // Legacy fields maintained for compatibility
        public int AvatarIndex { get; set; }
        public string BloodType { get; set; } = "";
        public int Role_ID { get; set; }
        public string Status { get; set; } = "";

        // Navigation Properties
        public virtual EmploymentDetails EmploymentDetails { get; set; }
        public virtual ContactInformation ContactInformation { get; set; }
        public virtual ICollection<EmergencyContact> EmergencyContacts { get; set; }
        public virtual ICollection<EmployeeDocument> EmployeeDocuments { get; set; }
        
        // Legacy navigation properties maintained for compatibility
        [ForeignKey("Role_ID")]
        public virtual Role role { get; set; }
        public ICollection<Exit> Exits { get; set; }
        public ICollection<Document> Documents { get; set; }
        public virtual GovernmentId GovernmentId { get; set; }
        public virtual CompanyProperty CompanyProperty { get; set; }
        public virtual ICollection<DocumentStatus> DocumentStatuses { get; set; }
    }
}