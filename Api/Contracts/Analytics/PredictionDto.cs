namespace Api.Contracts.Analytics
{
    public class PredictionDto
    {
        public int Id { get; set; }
        public string CandidateId { get; set; } = string.Empty;
        public double MatchScore { get; set; }
        public string ScreeningResult { get; set; } = string.Empty;
        public string ModelVersion { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
