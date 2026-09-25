using System.Threading.Channels;
using Microsoft.EntityFrameworkCore;
using Api.Contracts.Analytics;
using ApiHrm.Domains.Entities;
using ApiHrm.Infrastructures.Persistence;
using ApiHrm.Infrastructures.Persistence.Analytics;
using Applications.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
namespace Applications.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IAnalyticsRepository _analyticsRepository;
        private readonly hrmAppDbContext _hrmContext;
        private readonly AnalyticsDbContext _analyticsContext;
        private readonly Channel<int> _scoringQueue;
        private readonly ILogger<AnalyticsService> _logger;
        private readonly IDistributedCache _cache;

        public AnalyticsService(IAnalyticsRepository analyticsRepository, hrmAppDbContext hrmContext, AnalyticsDbContext analyticsContext, Channel<int> scoringQueue, ILogger<AnalyticsService> logger, IDistributedCache cache)
        {
            _analyticsRepository = analyticsRepository;
            _hrmContext = hrmContext;
            _analyticsContext = analyticsContext;
            _scoringQueue = scoringQueue;
            _logger = logger;
            _cache = cache;
        }

        private async Task<T> GetCachedAsync<T>(string cacheKey, Func<Task<T>> dataFactory, CancellationToken cancellationToken)
        {
            try
            {
                var cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);
                if (!string.IsNullOrEmpty(cachedData))
                {
                    return JsonSerializer.Deserialize<T>(cachedData)!;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to read from cache for key: {CacheKey}", cacheKey);
            }

            var data = await dataFactory();

            try
            {
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                };
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(data), options, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to write to cache for key: {CacheKey}", cacheKey);
            }

            return data;
        }

        public async Task<HrmsDashboardSummaryDto> GetHrmsDashboardSummaryAsync(CancellationToken cancellationToken = default)
        {
            return await GetCachedAsync("HrmsDashboardSummary", async () => 
            {
                try
            {
                var totalEmployees = await _hrmContext.Employees.CountAsync(cancellationToken);
                var regularEmployees = await _hrmContext.Employees.CountAsync(e => e.Status == "Regular", cancellationToken);
                
                var totalApplicants = await _hrmContext.Applicants.CountAsync(cancellationToken);
                var activeApplicants = await _hrmContext.Applicants.CountAsync(a => a.Hiring_Stage != "Hired" && a.Hiring_Stage != "Failed", cancellationToken);
                
                var totalAttendanceRecords = await _hrmContext.AttendanceLogs.CountAsync(cancellationToken);
                var onTimeCount = await _hrmContext.AttendanceLogs.CountAsync(a => a.Status == "On Time", cancellationToken);
                
                var totalPayroll = await _hrmContext.EmployeePayrollRecords.SumAsync(p => p.Net_Pay, cancellationToken);
                
                return new HrmsDashboardSummaryDto
                {
                    TotalEmployees = totalEmployees,
                    RegularEmployees = regularEmployees,
                    TotalApplicants = totalApplicants,
                    ActiveApplicants = activeApplicants,
                    TotalAttendanceRecords = totalAttendanceRecords,
                    OnTimeCount = onTimeCount,
                    TotalPayroll = totalPayroll
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching HRMS dashboard summary");
                return new HrmsDashboardSummaryDto();
            }
            }, cancellationToken);
        }

        public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default)
        {
            string cacheKey = $"DashboardSummary_{startDate?.ToString("yyyyMMdd")}_{endDate?.ToString("yyyyMMdd")}";
            return await GetCachedAsync(cacheKey, async () => 
            {
                try
            {
                var predictions = await _analyticsRepository.GetAllPredictionsAsync(cancellationToken);
                
                // Apply date filtering
                if (startDate.HasValue)
                {
                    predictions = predictions.Where(p => p.CreatedAt >= startDate.Value).ToList();
                }
                if (endDate.HasValue)
                {
                    predictions = predictions.Where(p => p.CreatedAt <= endDate.Value.AddDays(1)).ToList();
                }
                
                var totalScored = predictions.Count;
                
                // Get total applicants from PostgreSQL for accurate count
                var totalApplicants = await _hrmContext.Applicants.CountAsync(cancellationToken);
                
                if (totalScored == 0)
                {
                    return new DashboardSummaryDto { TotalApplicants = totalApplicants };
                }

                var qualified = predictions.Count(p => p.ScreeningResult == "Qualified");
                var review = predictions.Count(p => p.ScreeningResult == "Review");
                var notQualified = predictions.Count(p => p.ScreeningResult == "Not Qualified");

                return new DashboardSummaryDto
                {
                    TotalApplicants = totalApplicants,
                    TotalScored = totalScored,
                    QualifiedCount = qualified,
                    ReviewCount = review,
                    NotQualifiedCount = notQualified,
                    QualifiedRate = totalScored > 0 ? Math.Round((double)qualified / totalScored * 100, 2) : 0,
                    ReviewRate = totalScored > 0 ? Math.Round((double)review / totalScored * 100, 2) : 0,
                    NotQualifiedRate = totalScored > 0 ? Math.Round((double)notQualified / totalScored * 100, 2) : 0,
                    AverageMatchScore = Math.Round(predictions.Select(p => p.MatchScore).DefaultIfEmpty(0).Average(), 2),
                    LastScoredAt = predictions.Max(p => p.CreatedAt)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching dashboard summary from analytics repository");
                return new DashboardSummaryDto();
            }
            }, cancellationToken);
        }

        public async Task<ScoreDistributionDto> GetScoreDistributionAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default)
        {
            string cacheKey = $"ScoreDistribution_{startDate?.ToString("yyyyMMdd")}_{endDate?.ToString("yyyyMMdd")}";
            return await GetCachedAsync(cacheKey, async () => 
            {
                try
            {
                var predictions = await _analyticsRepository.GetAllPredictionsAsync(cancellationToken);
                
                // Apply date filtering
                if (startDate.HasValue)
                {
                    predictions = predictions.Where(p => p.CreatedAt >= startDate.Value).ToList();
                }
                if (endDate.HasValue)
                {
                    predictions = predictions.Where(p => p.CreatedAt <= endDate.Value.AddDays(1)).ToList();
                }
                var bucketRanges = new List<(string Label, double Min, double Max)>
                {
                    ("90-100", 90, 100),
                    ("70-89", 70, 89.99),
                    ("46-69", 46, 69.99),
                    ("30-45", 30, 45.99),
                    ("0-29", 0, 29.99)
                };

                var distribution = new ScoreDistributionDto();
                foreach (var (label, min, max) in bucketRanges)
                {
                    var count = predictions.Count(p => p.MatchScore >= min && p.MatchScore <= max);
                    distribution.Buckets.Add(new ScoreBucketDto
                    {
                        Label = label,
                        Min = min,
                        Max = max,
                        Count = count
                    });
                }

                return distribution;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error fetching score distribution from analytics repository");
                return new ScoreDistributionDto { Buckets = new List<ScoreBucketDto>() };
            }
            }, cancellationToken);
        }

        public async Task<IReadOnlyList<TopCandidateDto>> GetTopCandidatesAsync(int count, DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var predictions = await _analyticsRepository.GetAllPredictionsAsync(cancellationToken);
                
                // Apply date filtering
                if (startDate.HasValue)
                {
                    predictions = predictions.Where(p => p.CreatedAt >= startDate.Value).ToList();
                }
                if (endDate.HasValue)
                {
                    predictions = predictions.Where(p => p.CreatedAt <= endDate.Value.AddDays(1)).ToList();
                }
                
                var topPredictions = predictions.OrderByDescending(p => p.MatchScore).Take(count).ToList();

                var candidateIds = topPredictions
                    .Select(p => int.TryParse(p.CandidateId, out var id) ? (int?)id : null)
                    .Where(id => id.HasValue)
                    .Select(id => id!.Value)
                    .Distinct()
                    .ToList();

                var applicants = await _hrmContext.Applicants
                    .AsNoTracking()
                    .Where(a => candidateIds.Contains(a.Applicant_ID) && 
                               a.Hiring_Stage != "Failed")
                    .ToDictionaryAsync(a => a.Applicant_ID, cancellationToken);

                var result = new List<TopCandidateDto>();
                foreach (var prediction in topPredictions)
                {
                    if (!int.TryParse(prediction.CandidateId, out var applicantId))
                        continue;

                    // INNER JOIN behavior: exclude if applicant not found
                    if (!applicants.ContainsKey(applicantId))
                        continue;

                    var applicant = applicants[applicantId];

                    result.Add(new TopCandidateDto
                    {
                        ApplicantId = applicantId,
                        FirstName = applicant.First_Name,
                        LastName = applicant.Last_Name,
                        Position = applicant.Position ?? "Unspecified",
                        MatchScore = prediction.MatchScore,
                        ScreeningResult = prediction.ScreeningResult,
                        ModelVersion = prediction.ModelVersion,
                        CreatedAt = prediction.CreatedAt
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error fetching top candidates from PostgreSQL applicants");
                return new List<TopCandidateDto>();
            }
        }

        public async Task<IReadOnlyList<PositionFitDto>> GetPositionFitAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default)
        {
            string cacheKey = $"PositionFit_{startDate?.ToString("yyyyMMdd")}_{endDate?.ToString("yyyyMMdd")}";
            return await GetCachedAsync(cacheKey, async () => 
            {
                try
            {
                var predictions = await _analyticsRepository.GetAllPredictionsAsync(cancellationToken);

                // Apply date filtering
                if (startDate.HasValue)
                {
                    predictions = predictions.Where(p => p.CreatedAt >= startDate.Value).ToList();
                }
                if (endDate.HasValue)
                {
                    predictions = predictions.Where(p => p.CreatedAt <= endDate.Value.AddDays(1)).ToList();
                }

                var validPredictions = predictions
                    .Where(p => int.TryParse(p.CandidateId, out _))
                    .ToList();

                if (validPredictions.Count == 0)
                {
                    return new List<PositionFitDto>();
                }

                var candidateIds = validPredictions
                    .Select(p => int.Parse(p.CandidateId))
                    .Distinct()
                    .ToList();

                var applicants = await _hrmContext.Applicants
                    .AsNoTracking()
                    .Where(a => candidateIds.Contains(a.Applicant_ID))
                    .ToDictionaryAsync(a => a.Applicant_ID, cancellationToken);

                var grouped = validPredictions
                    .GroupBy(p =>
                    {
                        var id = int.Parse(p.CandidateId);
                        return applicants.ContainsKey(id)
                            ? (applicants[id].Position ?? "Unspecified")
                            : "Unspecified";
                    });

                var result = new List<PositionFitDto>();
                foreach (var group in grouped)
                {
                    var values = group.ToList();
                    var avg = values.Select(p => p.MatchScore).DefaultIfEmpty(0).Average();
                    var qualified = values.Count(p => p.ScreeningResult == "Qualified");
                    var review = values.Count(p => p.ScreeningResult == "Review");
                    var notQualified = values.Count(p => p.ScreeningResult == "Not Qualified");

                    var fit = avg switch
                    {
                        < 30 => "Skill Gap / Low Fit",
                        < 46 => "Moderate Fit",
                        _ => "Strong Fit"
                    };

                    result.Add(new PositionFitDto
                    {
                        Position = group.Key,
                        TotalApplicants = values.Count,
                        AverageMatchScore = Math.Round(avg, 2),
                        QualifiedCount = qualified,
                        ReviewCount = review,
                        NotQualifiedCount = notQualified,
                        FitIndication = fit
                    });
                }

                return result.OrderBy(p => p.AverageMatchScore).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching position fit data from PostgreSQL applicants");
                return new List<PositionFitDto>();
            }
            }, cancellationToken);
        }

        public async Task<IReadOnlyList<PredictionDto>> GetPredictionsAsync(int page = 1, int pageSize = 50, CancellationToken cancellationToken = default)
        {
            try
            {
                var predictions = await _analyticsRepository.GetAllPredictionsAsync(cancellationToken);
                return predictions
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new PredictionDto
                    {
                        Id = p.Id,
                        CandidateId = p.CandidateId,
                        MatchScore = p.MatchScore,
                        ScreeningResult = p.ScreeningResult,
                        ModelVersion = p.ModelVersion,
                        CreatedAt = p.CreatedAt
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error fetching predictions from analytics repository");
                return new List<PredictionDto>();
            }
        }

        public async Task<int> RescoreAllApplicantsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                // Clean slate: wipe old predictions before rescoring
                _analyticsContext.Predictions.RemoveRange(_analyticsContext.Predictions);
                await _analyticsContext.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Cleared all existing predictions for clean slate rescore");

                var applicants = await _hrmContext.Applicants
                    .AsNoTracking()
                    .Select(a => a.Applicant_ID)
                    .ToListAsync(cancellationToken);

                int enqueuedCount = 0;
                foreach (var applicantId in applicants)
                {
                    if (_scoringQueue.Writer.TryWrite(applicantId))
                    {
                        enqueuedCount++;
                    }
                }

                return enqueuedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rescoring applicants - failed to fetch from PostgreSQL");
                return 0;
            }
        }

        public async Task<IReadOnlyList<ApplicationTrendDto>> GetApplicationTrendsAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default)
        {
            string cacheKey = $"ApplicationTrends_{startDate?.ToString("yyyyMMdd")}_{endDate?.ToString("yyyyMMdd")}";
            return await GetCachedAsync(cacheKey, async () => 
            {
                try
            {
                // Pure dynamic data floor: if startDate is null, get earliest date from database
                if (!startDate.HasValue)
                {
                    var earliestDate = await _hrmContext.Applicants
                        .AsNoTracking()
                        .MinAsync(a => (DateOnly?)a.Application_Date, cancellationToken);

                    if (earliestDate.HasValue)
                    {
                        startDate = earliestDate.Value.ToDateTime(TimeOnly.MinValue);
                    }
                    else
                    {
                        startDate = DateTime.UtcNow.AddMonths(-1);
                    }
                }

                var predictions = await _analyticsRepository.GetAllPredictionsAsync(cancellationToken);

                // Get valid candidate IDs
                var validPredictions = predictions
                    .Where(p => int.TryParse(p.CandidateId, out _))
                    .ToList();

                var candidateIds = validPredictions
                    .Select(p => int.Parse(p.CandidateId))
                    .Distinct()
                    .ToList();

                var applicants = await _hrmContext.Applicants
                    .AsNoTracking()
                    .Where(a => candidateIds.Contains(a.Applicant_ID))
                    .Select(a => new { a.Applicant_ID, a.Position, a.Application_Date })
                    .ToListAsync(cancellationToken);

                var applicantData = applicants.ToDictionary(a => a.Applicant_ID, a => new { a.Position, a.Application_Date });

                // Filter predictions based on Application_Date
                var filteredPredictions = validPredictions
                    .Where(p => int.TryParse(p.CandidateId, out var id) && applicantData.ContainsKey(id))
                    .ToList();

                // Apply date filtering based on Application_Date
                if (startDate.HasValue)
                {
                    filteredPredictions = filteredPredictions
                        .Where(p => applicantData[int.Parse(p.CandidateId)].Application_Date >= DateOnly.FromDateTime(startDate.Value))
                        .ToList();
                }
                if (endDate.HasValue)
                {
                    filteredPredictions = filteredPredictions
                        .Where(p => applicantData[int.Parse(p.CandidateId)].Application_Date <= DateOnly.FromDateTime(endDate.Value))
                        .ToList();
                }

                // Group by date and position for flat list with normalization
                var trends = filteredPredictions
                    .GroupBy(p => new
                    {
                        Date = applicantData[int.Parse(p.CandidateId)].Application_Date,
                        Role = NormalizePosition(applicantData[int.Parse(p.CandidateId)].Position ?? "Unspecified")
                    })
                    .Select(g => new ApplicationTrendDto
                    {
                        Date = g.Key.Date.ToString("yyyy-MM-dd"),
                        Role = g.Key.Role,
                        Count = g.Count()
                    })
                    .OrderBy(t => t.Date)
                    .ToList();

                return trends;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching application trends from analytics repository");
                return new List<ApplicationTrendDto>();
            }
            }, cancellationToken);
        }

        private string NormalizePosition(string position)
        {
            if (string.IsNullOrEmpty(position))
                return "Unspecified";

            return position.Replace("Intern / Summer Job", "Intern/Summer Job").Trim();
        }

        public async Task<IReadOnlyList<TopCandidateDto>> GetAllCandidatesForExportAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var predictions = await _analyticsRepository.GetAllPredictionsAsync(cancellationToken);

                // Apply date filtering if provided
                if (startDate.HasValue)
                {
                    predictions = predictions.Where(p => p.CreatedAt >= startDate.Value).ToList();
                }
                if (endDate.HasValue)
                {
                    predictions = predictions.Where(p => p.CreatedAt <= endDate.Value.AddDays(1)).ToList();
                }

                var candidateIds = predictions
                    .Select(p => int.TryParse(p.CandidateId, out var id) ? (int?)id : null)
                    .Where(id => id.HasValue)
                    .Select(id => id!.Value)
                    .Distinct()
                    .ToList();

                var applicants = await _hrmContext.Applicants
                    .AsNoTracking()
                    .Where(a => candidateIds.Contains(a.Applicant_ID) &&
                               a.Hiring_Stage != "Failed")
                    .ToDictionaryAsync(a => a.Applicant_ID, cancellationToken);

                var result = new List<TopCandidateDto>();
                foreach (var prediction in predictions)
                {
                    if (!int.TryParse(prediction.CandidateId, out var applicantId))
                        continue;

                    // INNER JOIN behavior: exclude if applicant not found
                    if (!applicants.ContainsKey(applicantId))
                        continue;

                    var applicant = applicants[applicantId];

                    string rawPhone = applicant.Mobile ?? applicant.Contact_Details ?? "";
                    string phoneNumber = rawPhone.Length > 20 ? "" : rawPhone;

                    result.Add(new TopCandidateDto
                    {
                        ApplicantId = applicantId,
                        FirstName = applicant.First_Name,
                        LastName = applicant.Last_Name,
                        Email = applicant.Email ?? "",
                        PhoneNumber = phoneNumber,
                        Position = applicant.Position ?? "Unspecified",
                        MatchScore = prediction.MatchScore,
                        ScreeningResult = prediction.ScreeningResult,
                        ModelVersion = prediction.ModelVersion,
                        CreatedAt = prediction.CreatedAt
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error fetching all candidates for export from PostgreSQL applicants");
                return new List<TopCandidateDto>();
            }
        }
    }
}
