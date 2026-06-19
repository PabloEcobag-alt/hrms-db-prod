namespace Api.Contracts.Digital201
{
    public class AdminEmployeeListDto
    {
        public int EmployeeId { get; set; }
        public string? ErpUserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
    }
}
