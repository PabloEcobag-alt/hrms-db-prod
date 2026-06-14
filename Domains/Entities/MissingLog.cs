using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiHrm.Domains.Entities
{
    [Table("r_Missing_Log")]
    public class MissingLog
    {
        [Key]
        [Column("missing_log_id")]
        public int Id { get; set; }

        [Required]
        public int Employee_Id { get; set; }

        [Required]
        public DateOnly Log_Date { get; set; }

        public string Reason { get; set; }

        [ForeignKey("Employee_Id")]
        public virtual Employee Employee { get; set; }
    }
}
