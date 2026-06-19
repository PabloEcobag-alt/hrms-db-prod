namespace Api.Contracts.Applicant
{
    public class EcommerceApplicationDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string? ResumeFileName { get; set; }
        public string? CoverLetter { get; set; }
        public string? ExperienceJson { get; set; }
        public string? EducationJson { get; set; }
        public string? Skills { get; set; }
    }
}
