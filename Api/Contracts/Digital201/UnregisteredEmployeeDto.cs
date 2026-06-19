namespace Api.Contracts.Digital201
{
    public class UnregisteredEmployeeDto
    {
        public int employeeId { get; set; }
        public string firstName { get; set; } = string.Empty;
        public string lastName { get; set; } = string.Empty;
        public string emailAddress { get; set; } = string.Empty;
        public string department { get; set; } = string.Empty;
        public string position { get; set; } = string.Empty;
    }
}
