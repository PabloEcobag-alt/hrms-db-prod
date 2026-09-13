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

        // JSONB Snapshot Properties for Automatic Audit Interceptor
        public string? OldValues { get; set; }  // Serialized JSON of original entity state
        public string? NewValues { get; set; }  // Serialized JSON of current entity state
        public string Operation { get; set; } = string.Empty;  // INSERT, UPDATE, DELETE
        public string EntityName { get; set; } = string.Empty;  // Entity type name
    }
}
