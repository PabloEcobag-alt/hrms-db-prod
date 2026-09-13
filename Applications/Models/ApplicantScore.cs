using System.Text.Json.Serialization;

namespace Applications.Models
{
    /// <summary>
    /// Result returned by any scoring provider (OpenAI or local ML).
    /// Kept in the Application layer so it can be shared between
    /// scoring services and the analytics persistence layer.
    /// </summary>
    public class ApplicantScore
    {
        [JsonPropertyName("match_score")]
        public double MatchScore { get; set; }

        [JsonPropertyName("screening_result")]
        public string ScreeningResult { get; set; } = "Not Qualified";

        [JsonPropertyName("model_version")]
        public string ModelVersion { get; set; } = string.Empty;
    }
}
