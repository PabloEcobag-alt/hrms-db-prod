using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiHrm.Domains.Entities
{
    [Table("r_Attendance_Log")]
    public class AttendanceLog
    {
        [Key]
        [Column("attendance_id")]
        public int Id { get; set; }

        [Required]
        public int Employee_Id { get; set; }

        [Required]
        public DateOnly Log_Date { get; set; }

        public DateTime? Time_In { get; set; }

        public DateTime? Time_Out { get; set; }

        public string Status { get; set; }

        public string Log_Source { get; set; }

        public bool Is_Manual_Override { get; set; }

        [ForeignKey("Employee_Id")]
        public virtual Employee Employee { get; set; }
    }
}
