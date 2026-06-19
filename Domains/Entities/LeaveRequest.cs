using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiHrm.Domains.Entities
{
    [Table("r_Leave_Request")]
    public class LeaveRequest
    {
        [Key]
        [Column("leave_request_id")]
        public int Id { get; set; }

        [Required]
        public int Employee_Id { get; set; }

        [Required]
        public int LeaveTypeId { get; set; }

        [Required]
        public DateOnly Start_Date { get; set; }

        [Required]
        public DateOnly End_Date { get; set; }

        public string Reason { get; set; }

        public string Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("Employee_Id")]
        public virtual Employee Employee { get; set; }

        [ForeignKey("LeaveTypeId")]
        public virtual LeaveType LeaveType { get; set; }
    }
}
