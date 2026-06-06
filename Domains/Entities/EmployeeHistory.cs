using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiHrm.Domains.Entities
{
    [Table("r_Employee_History")]

    public class EmployeeHistory
    {
        [Key]
        public int History_ID { get; set; }
        public int Employee_ID { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Action_Type { get; set; }

        [MaxLength(255)]
        public string? Old_Value { get; set; }

        [MaxLength(255)]
        public string? New_Value { get; set; }

        [Required]
        public required string Changed_By { get; set; }
        public DateTime Changed_At { get; set; } = DateTime.UtcNow;

        [ForeignKey("Employee_ID")]
        public virtual Employee Employee { get; set; }
    }
}
