namespace Api.Contracts.Employee
{
    public class EmployeeReadDto
    {
        public int Employee_Id { get; set; }
        public string FullName { get; set; } = default!;
        public string Position { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string RoleName { get; set; } = default!;
        public string Status { get; set; } = default!;
        public DateTime Hire_Date { get; set; }
    }
}