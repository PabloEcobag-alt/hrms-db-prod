using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiHrm.Domains.Entities
{
    [Table("r_Leave_Type")]
    public class LeaveType
    {
        [Key]
        public int LeaveTypeId { get; set; }

        [Required]
        public required string LeaveTypeName { get; set; }
        public string? Description { get; set; }

        public virtual ICollection<LeaveBalance> LeaveBalances { get; set; } = new List<LeaveBalance>();
    }
}
