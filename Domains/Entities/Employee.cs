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
        public DateTime Hire_Date { get; set; }
        public string Email { get; set; }
        public string Contact_Details { get; set; }
        public string Payment_Method { get; set; }
        public int Role_ID { get; set; }
        public string Status { get; set; }

        [ForeignKey("Role_ID")]
        public virtual Role role { get; set; }
        public ICollection<Exit> Exits { get; set; }
        public ICollection<Document> Documents { get; set; }
    }
}