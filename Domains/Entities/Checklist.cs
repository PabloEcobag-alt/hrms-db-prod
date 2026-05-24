using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiHrm.Domains.Entities
{
    [Table("r_Checklist")]
    public class Checklist
    {
        [Key]
        public int Checklist_ID { get; set; }
        public int Applicant_ID { get; set; }
        public bool Has_NBI { get; set; }
        public bool Has_Medical { get; set; }
        public bool Has_Xray { get; set; }
        public bool has_SSS { get; set; }
        public bool has_PAGIBIG { get; set; }
        public bool has_PhilHealth { get; set; }
        public bool has_TIN { get; set; }

        [ForeignKey("Applicant_ID")]
        public virtual Applicant Applicant { get; set; }
    }
}