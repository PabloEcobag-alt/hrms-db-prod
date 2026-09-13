using System.ComponentModel.DataAnnotations;

namespace Api.Contracts.Applicant
{
    public class ApplicantCreateDto
    {
        [Required(ErrorMessage = "First name is required")]
        [StringLength(50)]
        public string firstName { get; set; } = default!;

        [StringLength(50)]
        public string? middleName { get; set; }

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

        [RegularExpression(@"^\+63\d{10}$", ErrorMessage = "Invalid Philippine mobile format")]
        public string mobile { get; set; } = default!;

        public DateOnly? interviewDate { get; set; }

        public DateOnly? expectedStart { get; set; }

        public DateOnly? probationaryEndDate { get; set; }

        public string hiringStage { get; set; } = "Initial Interview"; // Default hiring stage

        // Legacy fields for backward compatibility
        public string? Contact_Details { get; set; }
        public string? Payment_Method { get; set; }
        public string Resume_URL { get; set; } = default!;
        public string Status { get; set; } = "Applied";
    }
}