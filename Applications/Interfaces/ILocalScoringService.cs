using Applications.Models;

namespace Applications.Interfaces
{
    /// <summary>
    /// Marker interface for the local Python ML-based applicant scorer.
    /// Inherits the shared scoring contract so it can be injected as
    /// the default <see cref="IScoringService"/> implementation.
    /// </summary>
    public interface ILocalScoringService : IScoringService
    {
    }
}
