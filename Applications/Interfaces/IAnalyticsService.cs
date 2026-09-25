using Api.Contracts.Analytics;

namespace Applications.Interfaces
{
    /// <summary>
    /// Service that aggregates AI scoring and vector search data for the ARAE dashboard.
    /// </summary>
    public interface IAnalyticsService
    {
        Task<HrmsDashboardSummaryDto> GetHrmsDashboardSummaryAsync(CancellationToken cancellationToken = default);
        Task<DashboardSummaryDto> GetDashboardSummaryAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
        Task<ScoreDistributionDto> GetScoreDistributionAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TopCandidateDto>> GetTopCandidatesAsync(int count, DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<PositionFitDto>> GetPositionFitAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<PredictionDto>> GetPredictionsAsync(int page = 1, int pageSize = 50, CancellationToken cancellationToken = default);
        Task<int> RescoreAllApplicantsAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ApplicationTrendDto>> GetApplicationTrendsAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TopCandidateDto>> GetAllCandidatesForExportAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
    }
}
