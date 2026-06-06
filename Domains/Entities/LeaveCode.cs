using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiHrm.Domains.Entities
{
    [Table("r_Leave_Code")]

    public class LeaveCode
    {
        [Key]
        public int Leave_Code_ID { get; set; }

        [Required]
        public required string Leave_Code { get; set; }
        public string? Description { get; set; }

        public virtual ICollection<LeaveBalance> LeaveBalances { get; set; } = new List<LeaveBalance>();
    }
}
