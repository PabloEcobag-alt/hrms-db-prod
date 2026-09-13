namespace Api.Contracts.Analytics
{
    public class PositionFitDto
    {
        public string Position { get; set; } = string.Empty;
        public int TotalApplicants { get; set; }
        public double AverageMatchScore { get; set; }
        public int QualifiedCount { get; set; }
        public int ReviewCount { get; set; }
        public int NotQualifiedCount { get; set; }
        public string FitIndication { get; set; } = string.Empty;
    }
}
