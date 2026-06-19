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
        public string Position { get; set; } // Applied position
        public string Email { get; set; } 
        public string? Contact_Details { get; set; }
        public string? Payment_Method { get; set; } 
        public string? Resume_URL { get; set; } 
        public string Status { get; set; } // Expected: "Training" | "Probationary" | "Regular"
        
        // Hiring Information
        public string Hiring_Stage { get; set; } // Expected: "For Interview" | "Hired" | "Requirement Walking"
        public string Source { get; set; } // Expected: "Summer Job" | "Walk-In" | "Gmail Submission" | "Website Portal"
        public bool NBI_Document_Completed { get; set; }
        public bool Medical_Document_Completed { get; set; }
        public bool XRay_Document_Completed { get; set; }
        public string Phone { get; set; }
        
        public DateOnly Application_Date { get; set; } 
        public DateOnly? Interview_Date { get; set; }
        public DateOnly? Expected_Start_Date { get; set; }
        
        public virtual ICollection<Document>Documents { get; set; }
        public virtual ICollection <Checklist>Checklists { get; set; }
    }
}