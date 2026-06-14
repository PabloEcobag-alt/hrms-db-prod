using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiHrm.Domains.Entities
{
    [Table("r_Employee_Shift")]
    public class EmployeeShift
    {
        [Key]
        [Column("shift_id")]
        public int Id { get; set; }

        [Required]
        public int Employee_Id { get; set; }

        [Required]
        public string Shift_Code { get; set; }

        [Required]
        public DateOnly Effective_Date { get; set; }

        public DateOnly? End_Date { get; set; }

        [ForeignKey("Shift_Code")]
        public virtual ShiftDefinition ShiftDefinition { get; set; }

        [ForeignKey("Employee_Id")]
        public virtual Employee Employee { get; set; }
    }
}
