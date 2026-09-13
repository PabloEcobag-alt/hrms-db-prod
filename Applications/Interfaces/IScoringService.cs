using Applications.Models;

namespace Applications.Interfaces
{
    public interface IScoringService
    {
        /// <summary>
        /// Compares an applicant's profile against a job position and returns
        /// a structured score with match percentage (0-100), screening bucket,
        /// and the model version that produced it.
        /// </summary>
        Task<ApplicantScore> ScoreApplicantAsync(string position, string applicantProfile, string skills = null, string experience = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Identifier of the underlying model, stored alongside each prediction.
        /// </summary>
        string ModelVersion { get; }
    }
}
