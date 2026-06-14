using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiHrm.Domains.Entities
{
    [Table("r_Sss_Bracket")]
    public class SssBracket
    {
        [Key]
        [Column("sss_bracket_id")]
        public int Id { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SalaryRangeStart { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SalaryRangeEnd { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal EmployeeShareRate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal EmployerShareRate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal MonthlyContribution { get; set; }

        public int EffectiveYear { get; set; }
    }
}
