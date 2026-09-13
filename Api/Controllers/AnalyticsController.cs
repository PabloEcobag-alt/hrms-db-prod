using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Applications.Interfaces;
using Api.Contracts.Analytics;

namespace ApiHrm.Controllers
{
    [Route("api/hrms/analytics")]
    [ApiController]
    [Authorize]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly ILogger<AnalyticsController> _logger;

        public AnalyticsController(IAnalyticsService analyticsService, ILogger<AnalyticsController> logger)
        {
            _analyticsService = analyticsService;
            _logger = logger;
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardSummaryDto>> GetDashboardSummary(CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _analyticsService.GetDashboardSummaryAsync(cancellationToken));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Database unavailable for dashboard summary; returning empty state");
                return Ok(new DashboardSummaryDto());
            }
        }

        [HttpGet("score-distribution")]
        public async Task<ActionResult<ScoreDistributionDto>> GetScoreDistribution(CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _analyticsService.GetScoreDistributionAsync(cancellationToken));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Database unavailable for score distribution; returning empty state");
                return Ok(new ScoreDistributionDto { Buckets = new List<ScoreBucketDto>() });
            }
        }

        [HttpGet("top-candidates")]
        public async Task<ActionResult<IReadOnlyList<TopCandidateDto>>> GetTopCandidates(
            [FromQuery] int count = 10,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return Ok(await _analyticsService.GetTopCandidatesAsync(count, cancellationToken));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Database unavailable for top candidates; returning empty list");
                return Ok(new List<TopCandidateDto>());
            }
        }

        [HttpGet("position-fit")]
        public async Task<ActionResult<IReadOnlyList<PositionFitDto>>> GetPositionFit(CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _analyticsService.GetPositionFitAsync(cancellationToken));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Database unavailable for position fit; returning empty list");
                return Ok(new List<PositionFitDto>());
            }
        }

        [HttpGet("predictions")]
        public async Task<ActionResult<IReadOnlyList<PredictionDto>>> GetPredictions(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return Ok(await _analyticsService.GetPredictionsAsync(page, pageSize, cancellationToken));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Database unavailable for predictions; returning empty list");
                return Ok(new List<PredictionDto>());
            }
        }

        [HttpPost("rescore")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> RescoreAllApplicants(CancellationToken cancellationToken)
        {
            try
            {
                var enqueuedCount = await _analyticsService.RescoreAllApplicantsAsync(cancellationToken);
                return Ok(new { message = "Rescore queued successfully", count = enqueuedCount });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Database unavailable for rescore; returning zero count");
                return Ok(new { message = "Database unavailable", count = 0 });
            }
        }

        [HttpGet("application-trends")]
        public async Task<ActionResult<IReadOnlyList<ApplicationTrendDto>>> GetApplicationTrends(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return Ok(await _analyticsService.GetApplicationTrendsAsync(startDate, endDate, cancellationToken));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Database unavailable for application trends; returning empty list");
                return Ok(new List<ApplicationTrendDto>());
            }
        }

        [HttpGet("export-candidates")]
        public async Task<ActionResult<IReadOnlyList<TopCandidateDto>>> GetAllCandidatesForExport(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return Ok(await _analyticsService.GetAllCandidatesForExportAsync(startDate, endDate, cancellationToken));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Database unavailable for candidate export; returning empty list");
                return Ok(new List<TopCandidateDto>());
            }
        }
    }
}
