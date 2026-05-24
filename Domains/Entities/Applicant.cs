using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiHrm.Domains.Entities
{    
    [Table("r_Apllicant_Records")]

    public class Applicant
    {
        [Key]
        public int Applicant_ID { get; set; } 
        public string First_Name { get; set; } 
        public string Last_Name { get; set; } 
        public string Email { get; set; } 
        public string? Contact_Details { get; set; }
        public string? Payment_Method { get; set; } 
        public string Resume_URL { get; set; } 
        public string Status { get; set; }
        
        public DateTime Application_Date { get; set; } 
        public virtual ICollection<Document>Documents { get; set; }
        public virtual ICollection <Checklist>Checklists { get; set; }
    }
}