using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiHrm.Domains.Entities
{
    [Table("r_Employee_Record")]

    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Employee_Id { get; set; }
        [Required]
        [StringLength(100)]
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Position { get; set; }
        public string Department { get; set; }
        public DateOnly Hire_Date { get; set; }
        public string Email { get; set; }
        public string Contact_Details { get; set; }
        public string PaymentMethod { get; set; } = "ATM";
        public string? AccountNumber { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal MonthlyBasePay { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DailyRate { get; set; }
        public int Role_ID { get; set; }
        public string Status { get; set; }

        // Personal Information
        public int AvatarIndex { get; set; }
        public string Address { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string Gender { get; set; } // Expected: "Male" | "Female" | etc.
        public string CivilStatus { get; set; } // Expected: "Single" | "Married" | etc.
        public string BloodType { get; set; } // Expected: "A+" | "B+" | "O+" | etc.
        public string ErpUserId { get; set; } // Link to identity server (GUID)

        [ForeignKey("Role_ID")]
        public virtual Role role { get; set; }
        public ICollection<Exit> Exits { get; set; }
        public ICollection<Document> Documents { get; set; }

        // Navigation properties for new entities
        public virtual EmergencyContact EmergencyContact { get; set; }
        public virtual GovernmentId GovernmentId { get; set; }
        public virtual CompanyProperty CompanyProperty { get; set; }
        public virtual ICollection<DocumentStatus> DocumentStatuses { get; set; }
    }
}