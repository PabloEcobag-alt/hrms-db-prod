namespace ApiHrm.Infrastructures.Persistence.Analytics
{
    public class SummaryReport
    {
        public int Id { get; set; }
        public string ReportType { get; set; } = string.Empty;
        public DateTime ReportDate { get; set; }
        public string SummaryData { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
