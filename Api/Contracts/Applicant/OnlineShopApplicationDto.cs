using Microsoft.AspNetCore.Http;

namespace ApiHrm.Api.Contracts.Applicant
{
    public class OnlineShopApplicationDto
    {
        public string JobPostingId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? CoverLetter { get; set; }
        public IFormFile? ResumeFile { get; set; }
    }
}
