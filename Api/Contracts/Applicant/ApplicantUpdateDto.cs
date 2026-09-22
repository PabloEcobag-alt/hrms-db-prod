using System.ComponentModel.DataAnnotations;

namespace Api.Contracts.Applicant
{
    public class ApplicantUpdateDto
    {
        public string? First_Name { get; set; }
        public string? Middle_Name { get; set; }
        public string? Last_Name { get; set; }
        public string? Hiring_Stage { get; set; }
        public DateOnly? Interview_Date { get; set; }
        public DateOnly? Expected_Start_Date { get; set; }
        public DateOnly? Probationary_End_Date { get; set; }
        public string? Status { get; set; }
        public string? Position { get; set; }

        public string? Mobile { get; set; }

        public string? Email { get; set; }
        public string? Interview_Notes { get; set; }
        
        public Dictionary<string, bool>? Requirements { get; set; }
    }
}
