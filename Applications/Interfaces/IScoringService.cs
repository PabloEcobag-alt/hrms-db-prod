namespace Applications.Interfaces
{
    public interface IScoringService
    {
        /// <summary>
        /// Compares an applicant's profile against a job position and returns
        /// an AI-generated match percentage between 0 and 100.
        /// </summary>
        Task<double> ScoreApplicantAsync(string position, string applicantProfile, CancellationToken cancellationToken = default);

        /// <summary>
        /// Identifier of the underlying model, stored alongside each prediction.
        /// </summary>
        string ModelVersion { get; }
    }
}
