using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiHrm.Domains.Entities
{
    [Table("r_Audit_Log")]
    public class AuditLog
    {
        [Key]
        [Column("audit_log_id")]
        public int Id { get; set; }

        public string User_Id { get; set; }

        public string User_Role { get; set; }

        public string Action { get; set; }

        public string Module { get; set; }

        public string Record_Id { get; set; }

        public string Record_Label { get; set; }

        public string Ip_Address { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
