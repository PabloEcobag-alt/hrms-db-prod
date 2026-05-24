using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiHrm.Domains.Entities
{
    [Table("r_Document")]

    public class Document
    {
        [Key]
        public int Document_ID { get; set; }
        public int? Employee_ID { get; set; }
        public int? Applicant_ID { get; set; }
        public string file_URL { get; set; }
        public string Doc_Type { get; set; }
        public DateTime upload_date { get; set; } = DateTime.UtcNow;

        [ForeignKey("Employee_ID")]
        public virtual Employee employee { get; set; }

        [ForeignKey("Applicant_ID")]
        public virtual Applicant applicant { get; set; }

    }
}