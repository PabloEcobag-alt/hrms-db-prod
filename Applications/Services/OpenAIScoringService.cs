using System.Text.Json;
using Applications.Interfaces;
using Applications.Models;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace Applications.Services
{
    public class OpenAIScoringService : IScoringService
    {
        // Keep token costs low per the design brief.
        public const string ModelId = "gpt-4o-mini";

        private const string SystemPrompt =
            "You are a strict, objective HR recruitment scoring engine. " +
            "Compare the APPLICANT PROFILE against the JOB POSITION and rate how well the applicant matches. " +
            "Base the score on relevant skills, experience, and role fit. Be conservative: only strong, " +
            "directly-relevant matches approach 100. " +
            "Respond with ONLY a single minified JSON object and nothing else, in the exact form: " +
            "{\"score\": <integer 0-100>}. Do not include explanations, markdown, or code fences.";

        private readonly IChatCompletionService _chat;
        private readonly ILogger<OpenAIScoringService> _logger;

        public OpenAIScoringService(IChatCompletionService chat, ILogger<OpenAIScoringService> logger)
        {
            _chat = chat;
            _logger = logger;
        }

        public string ModelVersion => ModelId;

        public async Task<ApplicantScore> ScoreApplicantAsync(string position, string applicantProfile, CancellationToken cancellationToken = default)
        {
            var history = new ChatHistory();
            history.AddSystemMessage(SystemPrompt);
            history.AddUserMessage(
                $"JOB POSITION:\n{position}\n\n" +
                $"APPLICANT PROFILE:\n{applicantProfile}");

            var settings = new OpenAIPromptExecutionSettings
            {
                Temperature = 0,
                MaxTokens = 50
            };

            var response = await _chat.GetChatMessageContentAsync(history, settings, kernel: null, cancellationToken);
            var raw = response.Content?.Trim() ?? string.Empty;

            double score = 0;
            if (TryParseScore(raw, out var parsed))
            {
                score = Math.Clamp(parsed, 0, 100);
            }
            else
            {
                _logger.LogWarning("OpenAI scoring returned unparsable content: {Raw}", raw);
            }

            return new ApplicantScore
            {
                MatchScore = score,
                ScreeningResult = Classify(score),
                ModelVersion = ModelVersion
            };
        }

        private static string Classify(double score)
        {
            if (score >= 80) return "Qualified";
            if (score >= 60) return "Review";
            return "Not Qualified";
        }

        private static bool TryParseScore(string raw, out double score)
        {
            score = 0;
            if (string.IsNullOrWhiteSpace(raw)) return false;

            try
            {
                using var doc = JsonDocument.Parse(raw);
                if (doc.RootElement.TryGetProperty("score", out var scoreEl))
                {
                    if (scoreEl.ValueKind == JsonValueKind.Number && scoreEl.TryGetDouble(out score))
                        return true;
                    if (scoreEl.ValueKind == JsonValueKind.String && double.TryParse(scoreEl.GetString(), out score))
                        return true;
                }
            }
            catch (JsonException)
            {
                // Fall through to defensive handling below.
            }

            return false;
        }
    }
}
