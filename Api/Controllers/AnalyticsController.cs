using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Applications.Interfaces;
using Api.Contracts.Analytics;

namespace ApiHrm.Controllers
{
    [Route("api/hrms/analytics")]
    [ApiController]
    [Authorize(Policy = "AnalyticsCanRead")]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly ILogger<AnalyticsController> _logger;
        private readonly ApiHrm.Infrastructures.Persistence.hrmAppDbContext _context;

        public AnalyticsController(IAnalyticsService analyticsService, ILogger<AnalyticsController> logger, ApiHrm.Infrastructures.Persistence.hrmAppDbContext context)
        {
            _analyticsService = analyticsService;
            _logger = logger;
            _context = context;
        }

        [HttpGet("hrms-summary")]
        [Authorize(Policy = "AnalyticsCanRead")]
        public async Task<ActionResult<object>> GetHrmsDashboardSummary(CancellationToken cancellationToken)
        {
            try
            {
                // Filter out soft-deleted employees
                var totalEmployees = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.CountAsync(_context.Employees, e => e.Status == "Active", cancellationToken);
                var regularEmployees = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.CountAsync(_context.EmploymentDetails, e => e.EmploymentStatus == "Regular", cancellationToken);

                var totalApplicants = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.CountAsync(_context.Applicants, cancellationToken);
                var activeApplicants = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.CountAsync(_context.Applicants, a => a.Hiring_Stage != "Hired" && a.Hiring_Stage != "Failed", cancellationToken);

                // Return an anonymous object; ASP.NET Core automatically serializes to camelCase JSON
                return Ok(new
                {
                    totalEmployees,
                    regularEmployees,
                    totalApplicants,
                    activeApplicants,
                    onTimeCount = 0,
                    totalAttendanceRecords = 0,
                    totalPayroll = 0
                });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Database unavailable for HRMS dashboard summary; returning empty state");
                return Ok(new HrmsDashboardSummaryDto());
            }
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
