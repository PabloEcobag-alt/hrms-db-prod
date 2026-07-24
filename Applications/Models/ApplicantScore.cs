namespace Applications.Models
{
    /// <summary>
    /// Result returned by any scoring provider (OpenAI or local ML).
    /// Kept in the Application layer so it can be shared between
    /// scoring services and the analytics persistence layer.
    /// </summary>
    public class ApplicantScore
    {
        public double MatchScore { get; set; }
        public string ScreeningResult { get; set; } = "Not Qualified";
        public string ModelVersion { get; set; } = string.Empty;
    }
}
