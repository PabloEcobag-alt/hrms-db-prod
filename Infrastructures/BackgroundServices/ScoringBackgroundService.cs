using System.Threading.Channels;
using Applications.Interfaces;
using Applications.Models;
using ApiHrm.Domains.Entities;
using ApiHrm.Infrastructures.Persistence;
using ApiHrm.Infrastructures.Persistence.Analytics;
using Microsoft.EntityFrameworkCore;

namespace ApiHrm.Infrastructures.BackgroundServices
{
    /// <summary>
    /// Drains the in-memory scoring queue. For each applicant id it loads the
    /// record from PostgreSQL, scores it via OpenAI, and persists the result
    /// into the SQLite analytics database (Predictions table).
    /// </summary>
    public class ScoringBackgroundService : BackgroundService
    {
        private readonly Channel<int> _queue;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ScoringBackgroundService> _logger;

        public ScoringBackgroundService(
            Channel<int> queue,
            IServiceProvider serviceProvider,
            ILogger<ScoringBackgroundService> logger)
        {
            _queue = queue;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ScoringBackgroundService started; awaiting new applicants to score.");

            await foreach (var applicantId in _queue.Reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    await ProcessApplicantAsync(applicantId, stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to score applicant {ApplicantId}", applicantId);
                }
            }
        }

        private async Task ProcessApplicantAsync(int applicantId, CancellationToken cancellationToken)
        {
            // BackgroundService is a singleton; DbContexts are scoped, so create a scope per item.
            using var scope = _serviceProvider.CreateScope();
            var provider = scope.ServiceProvider;

            var hrmContext = provider.GetRequiredService<hrmAppDbContext>();
            var analyticsContext = provider.GetRequiredService<AnalyticsDbContext>();
            var scoringService = provider.GetRequiredService<IScoringService>();

            var applicant = await hrmContext.Applicants
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Applicant_ID == applicantId, cancellationToken);

            if (applicant == null)
            {
                _logger.LogWarning("Applicant {ApplicantId} not found in PostgreSQL; skipping scoring.", applicantId);
                return;
            }

            // Defense in depth: skip failed applicants
            if (applicant.Hiring_Stage == "Failed")
            {
                _logger.LogInformation("Applicant {ApplicantId} has Hiring_Stage 'Failed'; skipping scoring.", applicantId);
                return;
            }

            var position = string.IsNullOrWhiteSpace(applicant.Position) ? "Unspecified" : applicant.Position;
            var profile = BuildApplicantProfile(applicant);

            // LAYER 3: Enterprise AI Circuit Breaker (Quota Limit)
            // To prevent DoW (Denial of Wallet) attacks, we strictly cap OpenAI API calls.
            var today = DateTime.UtcNow.Date;
            var openAiUsageToday = await analyticsContext.Predictions
                .CountAsync(p => p.CreatedAt >= today && p.ModelVersion.StartsWith("gpt-"), cancellationToken);

            IScoringService activeScorer = scoringService;

            // If we exceed 10 API calls today, we automatically fallback to the local free ML model.
            // Temporarily commented out to score all 50 mock applicants
            /*
            if (openAiUsageToday >= 10 && scoringService.GetType().Name.Contains("OpenAI"))
            {
                _logger.LogWarning("AI Circuit Breaker tripped! Daily quota (10) exceeded. Falling back to LocalScoringService for Applicant {ApplicantId}.", applicantId);
                activeScorer = provider.GetRequiredService<ILocalScoringService>();
            }
            */

            var scoreResult = await activeScorer.ScoreApplicantAsync(
                position, 
                profile, 
                applicant.Skills,
                applicant.Experience,
                cancellationToken);

            // Upsert logic: check if prediction exists for this applicant
            var existing = await analyticsContext.Predictions
                .FirstOrDefaultAsync(p => p.CandidateId == applicantId.ToString(), cancellationToken);

            if (existing != null)
            {
                // Update existing prediction
                existing.MatchScore = scoreResult.MatchScore;
                existing.ScreeningResult = scoreResult.ScreeningResult;
                existing.ModelVersion = scoreResult.ModelVersion;
                existing.CreatedAt = DateTime.UtcNow;
            }
            else
            {
                // Add new prediction
                analyticsContext.Predictions.Add(new Prediction
                {
                    CandidateId = applicantId.ToString(),
                    MatchScore = scoreResult.MatchScore,
                    ScreeningResult = scoreResult.ScreeningResult,
                    ModelVersion = scoreResult.ModelVersion,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await analyticsContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Scored applicant {ApplicantId}: {MatchScore}% match ({ScreeningResult}).", applicantId, scoreResult.MatchScore, scoreResult.ScreeningResult);
        }

        private static string BuildApplicantProfile(Applicant applicant)
        {
            var parts = new List<string>
            {
                $"Name: {applicant.First_Name} {applicant.Last_Name}",
                $"Applied Position: {applicant.Position}"
            };

            if (!string.IsNullOrWhiteSpace(applicant.Skills))
                parts.Add($"Skills: {applicant.Skills}");

            if (!string.IsNullOrWhiteSpace(applicant.Experience))
                parts.Add($"Experience: {applicant.Experience}");

            if (!string.IsNullOrWhiteSpace(applicant.Contact_Details))
                parts.Add($"Cover Letter / Details: {applicant.Contact_Details}");

            if (!string.IsNullOrWhiteSpace(applicant.Resume_URL))
                parts.Add($"Resume Reference: {applicant.Resume_URL}");

            return string.Join("\n", parts);
        }
    }
}
