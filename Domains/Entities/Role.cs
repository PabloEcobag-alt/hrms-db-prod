using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiHrm.Domains.Entities
{
    [Table("r_Role")]
    public class Role
    {
        [Key]
        public int Role_ID { get; set; }
        public string Role_Description { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
    }
}