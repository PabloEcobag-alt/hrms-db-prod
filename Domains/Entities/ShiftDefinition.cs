using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiHrm.Domains.Entities
{
    [Table("r_Shift_Definition")]
    public class ShiftDefinition
    {
        [Key]
        [MaxLength(50)]
        public string ShiftCode { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal StandardHours { get; set; } = 8.0m;

        public virtual ICollection<EmployeeShift> EmployeeShifts { get; set; }
    }
}
