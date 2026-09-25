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
            "You are an enterprise Applicant Tracking System (ATS) AI scoring engine. " +
            "Your objective is to strictly evaluate the APPLICANT PROFILE against the applied JOB POSITION. " +
            "Follow these strict rubrics for our specific roles:\n" +
            "- 'Senior Full-Stack Engineer (Next.js & .NET)': Heavily weight C#, .NET Core, React, Next.js, System Design, and 5+ years experience.\n" +
            "- 'Digital Marketing Specialist': Heavily weight SEO, Google Analytics, campaign management, copywriting, and e-commerce experience.\n" +
            "- 'Kitchen Operations Supervisor': Heavily weight food safety (HACCP), F&B supervisory experience, and inventory management.\n" +
            "- 'Commissary Helper': Heavily weight food preparation, kitchen cleanliness, stamina, and basic inventory tracking.\n" +
            "- 'Store Attendant' & 'On Call': Heavily weight customer service, cash handling/POS systems, communication, and reliability.\n" +
            "- 'Merchandiser On Call': Heavily weight visual display, restocking, product knowledge, and physical logistics.\n" +
            "- 'OJT (On-the-Job Trainee)' & 'Intern/Summer Job': Weight willingness to learn, basic computer/communication skills, and teamwork.\n" +
            "- Default/Other: Weight general experience and transferable skills.\n\n" +
            "You are evaluating for a fast-growing startup. Be objective but recognize potential and adaptability. " +
            "A score of 80-100 is a strong match ready to contribute immediately. 60-79 is a good match with solid transferable skills and room to grow. " +
            "40-59 means lacking some hard skills but highly trainable. Below 40 means a poor fit. " +
            "Respond with ONLY a single minified JSON object in the exact form: {\"score\": <integer 0-100>}. " +
            "Do not include explanations, markdown, or code fences.";

        private readonly IChatCompletionService _chat;
        private readonly ILogger<OpenAIScoringService> _logger;

        public OpenAIScoringService(IChatCompletionService chat, ILogger<OpenAIScoringService> logger)
        {
            _chat = chat;
            _logger = logger;
        }

        public string ModelVersion => ModelId;

        public async Task<ApplicantScore> ScoreApplicantAsync(string position, string applicantProfile, string skills = null, string experience = null, CancellationToken cancellationToken = default)
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
            if (score >= 46) return "Qualified";
            if (score >= 30) return "Review";
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
