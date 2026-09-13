using ApiHrm.Infrastructures.Persistence.Analytics;

namespace Applications.Interfaces
{
    /// <summary>
    /// Repository for reading AI prediction records from the SQLite analytics store.
    /// </summary>
    public interface IAnalyticsRepository
    {
        Task<IReadOnlyList<Prediction>> GetAllPredictionsAsync(CancellationToken cancellationToken = default);
        Task<int> GetPredictionCountAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Prediction>> GetTopPredictionsAsync(int count, CancellationToken cancellationToken = default);
    }
}
