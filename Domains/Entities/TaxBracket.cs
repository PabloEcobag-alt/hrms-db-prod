using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiHrm.Domains.Entities
{
    [Table("r_Tax_Bracket")]
    public class TaxBracket
    {
        [Key]
        [Column("tax_bracket_id")]
        public int Id { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal AnnualRangeStart { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal AnnualRangeEnd { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxRate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal BaseTax { get; set; }

        public int EffectiveYear { get; set; }
    }
}
