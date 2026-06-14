using System.ComponentModel.DataAnnotations;

namespace Api.Contracts.Employee
{
    public class EmployeeUpdateDto
    {
        public string? First_Name { get; set; }
        public string? Last_Name { get; set; }
        public string? Position { get; set; }
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }
        public string? Contact_Details { get; set; }
        public string? PaymentMethod { get; set; }
        public string? AccountNumber { get; set; }
        public string? Status { get; set; }
        public int? Role_Id { get; set; }
        public DateTime? Hire_Date { get; set; }
    }
}