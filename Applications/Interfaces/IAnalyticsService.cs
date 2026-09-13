using Api.Contracts.Analytics;

namespace Applications.Interfaces
{
    /// <summary>
    /// Service that aggregates AI scoring and vector search data for the ARAE dashboard.
    /// </summary>
    public interface IAnalyticsService
    {
        Task<DashboardSummaryDto> GetDashboardSummaryAsync(CancellationToken cancellationToken = default);
        Task<ScoreDistributionDto> GetScoreDistributionAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TopCandidateDto>> GetTopCandidatesAsync(int count, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<PositionFitDto>> GetPositionFitAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<PredictionDto>> GetPredictionsAsync(int page = 1, int pageSize = 50, CancellationToken cancellationToken = default);
        Task<int> RescoreAllApplicantsAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ApplicationTrendDto>> GetApplicationTrendsAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TopCandidateDto>> GetAllCandidatesForExportAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
    }
}
