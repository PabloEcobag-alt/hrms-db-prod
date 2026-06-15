using System.ComponentModel.DataAnnotations;

namespace Api.Contracts.Applicant
{
    public class ApplicantCreateDto
    {
        [Required(ErrorMessage = "First name is required")]
        [StringLength(50)]
        public string firstName { get; set; } = default!;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50)]
        public string lastName { get; set; } = default!;

        [Required]
        public string position { get; set; } = default!;

        [Required]
        public string source { get; set; } = default!; // Expected: "Summer Job" | "Walk-In" | "Gmail Submission" | "Website Portal"

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string email { get; set; } = default!;

        public string phone { get; set; } = default!;

        public string interviewDate { get; set; } = default!; // ISO format: YYYY-MM-DD

        public string expectedStart { get; set; } = default!; // ISO format: YYYY-MM-DD

        // Legacy fields for backward compatibility
        public string? Contact_Details { get; set; }
        public string? Payment_Method { get; set; }
        public string Resume_URL { get; set; } = default!;
        public string Status { get; set; } = "Applied";
    }
}