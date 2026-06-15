using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiHrm.Domains.Entities
{
    [Table("r_Company_Property")]
    public class CompanyProperty
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int Employee_Id { get; set; }

        [StringLength(20)]
        public string Employee_Id_Code { get; set; } // e.g., "EMP-001"

        public DateOnly Id_Issue_Date { get; set; }

        [StringLength(10)]
        public string Uniform_Top_Size { get; set; }

        [StringLength(10)]
        public string Uniform_Bottom_Size { get; set; }

        [StringLength(10)]
        public string Uniform_Shoe_Size { get; set; }

        public DateOnly Uniform_Issue_Date { get; set; }

        [Column(TypeName = "text")]
        public string Equipment_JSON { get; set; } // JSON array of equipment

        public DateOnly? Return_Date { get; set; }

        [ForeignKey("Employee_Id")]
        public virtual Employee Employee { get; set; }
    }
}
