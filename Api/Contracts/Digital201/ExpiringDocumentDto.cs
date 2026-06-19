namespace Api.Contracts.Digital201
{
    public class ExpiringDocumentDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public string DocumentName { get; set; } = string.Empty;
        public DateTime ExpirationDate { get; set; }
        public int DaysUntilExpiration { get; set; }
    }
}
