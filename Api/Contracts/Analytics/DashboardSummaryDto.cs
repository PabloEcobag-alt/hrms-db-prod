namespace Api.Contracts.Analytics
{
    public class DashboardSummaryDto
    {
        public int TotalApplicants { get; set; }
        public int TotalScored { get; set; }
        public int QualifiedCount { get; set; }
        public int ReviewCount { get; set; }
        public int NotQualifiedCount { get; set; }
        public double QualifiedRate { get; set; }
        public double ReviewRate { get; set; }
        public double NotQualifiedRate { get; set; }
        public double AverageMatchScore { get; set; }
        public DateTime? LastScoredAt { get; set; }
    }
}
