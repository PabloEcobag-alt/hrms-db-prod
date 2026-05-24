using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiHrm.Domains.Entities
{
    [Table("r_Exit_Interviews")]

    public class Exit
    {
        [Key]
        public int Exit_ID { get; set; }
        public int Employee_ID { get; set; }
        public string Reason_For_Leaving { get; set; }
        public string Interview_Notes { get; set; }
        public string Interviewed_By { get; set; }
        public DateTime Date { get; set; }
        [ForeignKey("Employee_ID")]
        public virtual Employee Employee { get; set; }
        

    }
}