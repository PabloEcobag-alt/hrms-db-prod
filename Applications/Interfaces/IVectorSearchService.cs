namespace Applications.Interfaces
{
    /// <summary>
    /// Contract for semantic candidate search via vector embeddings.
    /// Used by the ARAE module to find applicants matching natural language queries.
    /// </summary>
    public interface IVectorSearchService
    {
        /// <summary>
        /// Search for applicants matching a natural language query.
        /// Returns a list of candidate IDs with their similarity scores and positions.
        /// </summary>
        Task<List<CandidateSearchResult>> SearchCandidatesAsync(string query, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Result of a vector similarity search for a candidate.
    /// </summary>
    public class CandidateSearchResult
    {
        public int ApplicantId { get; set; }
        public string Position { get; set; } = string.Empty;
        public double SimilarityScore { get; set; }
        public double? Distance { get; set; }
    }
}
