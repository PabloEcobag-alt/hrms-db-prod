using Api.Contracts.Document;
using Api.Contracts.RoleExit;


namespace Api.Contracts.Employee
{
    public class EmployeeDetailDto
    {
        public int Employee_Id { get; set; }
        public string First_Name { get; set; } = default!;
        public string Last_Name { get; set; } = default!;
        public string Position { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Contact_Details { get; set; } = default!;
        public string PaymentMethod { get; set; } = default!;
        public string? AccountNumber { get; set; }
        public string Status { get; set; } = default!;
        public DateTime Hire_Date { get; set; }

        public int Role_Id { get; set; }
        public string Role_Name { get; set; } = default!;

        public List<DocumentReadDto> Documents { get; set; } = new();
        public List<ExitReadDto> Exits { get; set; } = new();
    }
}