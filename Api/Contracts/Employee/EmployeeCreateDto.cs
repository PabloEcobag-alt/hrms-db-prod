using System.ComponentModel.DataAnnotations;

namespace Api.Contracts.Employee
{
    public class EmployeeCreateDto
    {
        [Required(ErrorMessage = "First name is required")]
        [StringLength(50)]
        public string First_Name { get; set; } = default!;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50)]
        public string Last_Name { get; set; } = default!;

        [Required]
        public string Position { get; set; } = default!;

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = default!;

        public string? Contact_Details { get; set; }

        public string? Payment_Method { get; set; }

        [Required]
        [RegularExpression("^(Probationary|Regular|Resigned|Terminated|AWOL)$",
            ErrorMessage = "Status must be one of: Probationary, Regular, Resigned, Terminated, AWOL")]
        public string Status { get; set; } = "Probationary";

        [Required(ErrorMessage = "A role must be assigned")]
        public int Role_Id { get; set; }

        public DateTime Hire_Date { get; set; } = DateTime.UtcNow;
    }
}