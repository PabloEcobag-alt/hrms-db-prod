using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiHrm.Domains.Entities
{
    [Table("r_Leave_Balance")]

    public class LeaveBalance
    {
        [Key]
        public int Balance_ID { get; set; }
        public int Employee_ID { get; set; }
        public int Leave_Code_ID { get; set; }
        public int Allocated_Days { get; set; }
        public int Used_Days { get; set; }
        public int Pending_Days { get; set; }
        public int Year { get; set; }

        [ForeignKey("Employee_ID")]
        public virtual Employee Employee { get; set; }

        [ForeignKey("Leave_Code_ID")]
        public virtual LeaveCode LeaveCode { get; set; }
    }
}
