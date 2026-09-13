namespace Api.Contracts.Employee
{
    public class EmployeeReadDto
    {
        public int Employee_Id { get; set; }
        public string? FullName { get; set; }
        public string? Position { get; set; }
        public string? Email { get; set; }
        public string? RoleName { get; set; }
        public string? Status { get; set; }
        public DateTime Hire_Date { get; set; }
    }
}