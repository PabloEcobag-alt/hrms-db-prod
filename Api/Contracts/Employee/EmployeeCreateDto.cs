using System.ComponentModel.DataAnnotations;

namespace Api.Contracts.Employee
{
    public class EmployeeCreateDto
    {
        [Required(ErrorMessage = "First name is required")]
        [StringLength(50)]
        public string firstName { get; set; } = default!;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50)]
        public string lastName { get; set; } = default!;

        [Required]
        public string position { get; set; } = default!;

        public string department { get; set; } = default!;

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string email { get; set; } = default!;

        public string phone { get; set; } = default!;

        public string address { get; set; } = default!;

        public string dateOfBirth { get; set; } = default!; // ISO format: YYYY-MM-DD

        public string gender { get; set; } = default!;

        public string civilStatus { get; set; } = default!;

        public string bloodType { get; set; } = default!;

        public EmergencyContactDto emergencyContact { get; set; } = default!;

        public GovernmentIdsDto governmentIds { get; set; } = default!;

        public CompanyPropertyDto companyProperty { get; set; } = default!;

        public string paymentMethod { get; set; } = "Bank";

        public string? accountNumber { get; set; }

        [Required]
        public string status { get; set; } = "Active";

        [Required(ErrorMessage = "A role must be assigned")]
        public int roleId { get; set; }

        public string hireDate { get; set; } = default!; // ISO format: YYYY-MM-DD
    }
}