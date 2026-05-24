using System.ComponentModel.DataAnnotations;

namespace Api.Contracts.Applicant
{
    public class ApplicantCreateDto
    {
        [Required]
        public string First_Name { get; set; } = default!;
        [Required]
        public string Last_Name { get; set; } = default!;
        [Required, EmailAddress]
        public string Email { get; set; } = default!;
        [Required]
        public string? Contact_Details { get; set; }
        public string? Payment_Method { get; set; }
        public string Resume_URL { get; set; } = default!;
        public string Status { get; set; } = "Applied";
    }
}