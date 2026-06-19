using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiHrm.Domains.Entities
{
    [Table("r_Government_Id")]
    public class GovernmentId
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int Employee_Id { get; set; }

        [StringLength(20)]
        public string SSS_Number { get; set; }

        [StringLength(20)]
        public string PhilHealth_Number { get; set; }

        [StringLength(20)]
        public string TIN_Number { get; set; }

        [StringLength(20)]
        public string HDMF_Number { get; set; }

        public DateOnly? NbiClearanceDate { get; set; }
        public DateOnly? BarangayClearanceDate { get; set; }

        [ForeignKey("Employee_Id")]
        public virtual Employee Employee { get; set; }
    }
}
