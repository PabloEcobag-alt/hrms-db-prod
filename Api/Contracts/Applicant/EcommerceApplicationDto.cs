using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Api.Contracts.Applicant
{
    public class EcommerceApplicationDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        [RegularExpression(@"^\+63\d{10}$", ErrorMessage = "Invalid Philippine mobile format")]
        public string Mobile { get; set; } = string.Empty;

        public string Position { get; set; } = string.Empty;
        public IFormFile? ResumeFile { get; set; }
        public string? CoverLetter { get; set; }
        public string? ExperienceJson { get; set; }
        public string? EducationJson { get; set; }
        public string? Skills { get; set; }
    }
}
