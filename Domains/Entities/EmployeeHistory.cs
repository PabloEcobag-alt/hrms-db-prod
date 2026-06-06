using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiHrm.Domains.Entities
{
    [Table("r_Employee_History")]
    public class EmployeeHistory
    {
        [Key]
        [Column("History_ID")]
        public int History_ID { get; set; }
        
        [Required]
        [Column("Employee_ID")]
        public int Employee_ID { get; set; }
        
        [Required]
        [MaxLength(100)]
        [Column("Action_Type")]
        public string Action_Type { get; set; } = default!;
        
        [MaxLength(255)]
        [Column("Old_Value")]
        public string? Old_Value { get; set; }
        
        [MaxLength(255)]
        [Column("New_Value")]
        public string? New_Value { get; set; }
        
        [Required]
        [Column("Changed_By")]
        public string Changed_By { get; set; } = default!;
        
        [Required]
        [Column("Changed_At")]
        public DateTime Changed_At { get; set; }
        
        public virtual Employee Employee { get; set; } = default!;
    }
}
