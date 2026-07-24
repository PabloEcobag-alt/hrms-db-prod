namespace ApiHrm.Infrastructures.Persistence.Analytics
{
    public class Prediction
    {
        public int Id { get; set; }
        public string CandidateId { get; set; } = string.Empty;
        public double MatchScore { get; set; }
        public string ScreeningResult { get; set; } = "Not Qualified";
        public string ModelVersion { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
