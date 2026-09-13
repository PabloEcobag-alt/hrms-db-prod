using System.Diagnostics;
using System.Text.Json;
using Applications.Interfaces;
using Applications.Models;
using Microsoft.Extensions.Logging;

namespace Applications.Services
{
    /// <summary>
    /// Local ML scoring service that shells out to the Python ARAE tooling
    /// (arae-tooling/predict.py). Keeps all OpenAI-specific code isolated
    /// and runs deterministically without external API calls.
    /// </summary>
    public class LocalScoringService : ILocalScoringService
    {
        public const string DefaultModelVersion = "local-untrained";

        private readonly ILogger<LocalScoringService> _logger;
        private readonly string _toolingRoot;
        private readonly string _pythonExecutable;

        public LocalScoringService(ILogger<LocalScoringService> logger)
        {
            _logger = logger;

            _toolingRoot = Environment.GetEnvironmentVariable("ARAE_TOOLING_ROOT")
                ?? ResolveDefaultToolingRoot();

            _pythonExecutable = Environment.GetEnvironmentVariable("ARAE_PYTHON_EXECUTABLE")
                ?? "python3";

            ModelVersion = TryReadModelVersion();
        }

        public string ModelVersion { get; }

        public async Task<ApplicantScore> ScoreApplicantAsync(
            string position,
            string applicantProfile,
            string skills = null,
            string experience = null,
            CancellationToken cancellationToken = default)
        {
            var predictScript = Path.Combine(_toolingRoot, "predict.py");
            if (!File.Exists(predictScript))
            {
                throw new FileNotFoundException(
                    $"ARAE predict.py not found at '{predictScript}'. " +
                    "Set ARAE_TOOLING_ROOT or run from the expected repository layout.");
            }

            var tempInput = Path.GetTempFileName() + ".input.json";
            var tempOutput = Path.GetTempFileName() + ".output.json";

            try
            {
                // Use passed skills and experience if available, otherwise extract from profile
                // Provide safe fallback strings to prevent Python crashes
                var extractedSkills = !string.IsNullOrWhiteSpace(skills) ? skills : 
                                      !string.IsNullOrWhiteSpace(ExtractFieldFromProfile(applicantProfile, "Skills:")) ? 
                                      ExtractFieldFromProfile(applicantProfile, "Skills:") : "No skills provided";
                var extractedExperience = !string.IsNullOrWhiteSpace(experience) ? experience : 
                                            !string.IsNullOrWhiteSpace(ExtractFieldFromProfile(applicantProfile, "Experience:")) ? 
                                            ExtractFieldFromProfile(applicantProfile, "Experience:") : "No experience provided";

                var payload = new
                {
                    position = position ?? string.Empty,
                    contact_details = applicantProfile ?? string.Empty,
                    source = "Unknown",
                    skills = extractedSkills,
                    experience = extractedExperience
                };

                await File.WriteAllTextAsync(tempInput, JsonSerializer.Serialize(payload), cancellationToken);

                var startInfo = new ProcessStartInfo
                {
                    FileName = _pythonExecutable,
                    Arguments = $"\"{predictScript}\" --input \"{tempInput}\" --output \"{tempOutput}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = new Process { StartInfo = startInfo };
                process.Start();

                await process.WaitForExitAsync(cancellationToken);

                var stdout = await process.StandardOutput.ReadToEndAsync(cancellationToken);
                var stderr = await process.StandardError.ReadToEndAsync(cancellationToken);

                // Log raw stdout for debugging
                _logger.LogInformation("RAW Python stdout: {Stdout}", stdout);

                // Log stderr if not empty (indicates Python warnings/errors)
                if (!string.IsNullOrWhiteSpace(stderr))
                {
                    _logger.LogError("Python stderr output: {Stderr}", stderr);
                }

                if (process.ExitCode != 0)
                {
                    _logger.LogError("Local scoring failed with exit code {ExitCode}. Stderr: {Stderr}",
                        process.ExitCode, stderr);
                    throw new InvalidOperationException($"Local scoring failed: {stderr}");
                }

                _logger.LogDebug("Local scorer stdout: {Stdout}", stdout);

                if (!File.Exists(tempOutput))
                {
                    throw new InvalidOperationException("Local scorer did not produce an output file.");
                }

                var json = await File.ReadAllTextAsync(tempOutput, cancellationToken);
                
                // Log raw JSON from file for debugging
                _logger.LogInformation("RAW JSON from output file: {Json}", json);
                
                // Safe JSON parsing: extract JSON object from stdout if needed
                var jsonToParse = ExtractJsonFromOutput(json) ?? json;
                
                if (jsonToParse == null)
                {
                    _logger.LogError("Failed to extract JSON from output. Raw output: {RawOutput}", json);
                    throw new InvalidOperationException("Local scorer returned invalid output: no JSON found");
                }
                
                _logger.LogInformation("Attempting to deserialize JSON: {JsonToParse}", jsonToParse);
                
                var result = JsonSerializer.Deserialize<ApplicantScore>(jsonToParse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? throw new InvalidOperationException("Local scorer returned an empty or invalid score.");
                
                _logger.LogInformation("Successfully deserialized score: {MatchScore}, Result: {ScreeningResult}, Model: {ModelVersion}", 
                    result.MatchScore, result.ScreeningResult, result.ModelVersion);

                result.MatchScore = Math.Clamp(result.MatchScore, 0, 100);
                return result;
            }
            catch (Exception ex)
            {
                // Fallback prediction to ensure applicant is never dropped from database
                _logger.LogError(ex, "Local scoring failed, using fallback prediction");
                return new ApplicantScore
                {
                    MatchScore = 15.0,
                    ScreeningResult = "Not Qualified",
                    ModelVersion = "fallback-error"
                };
            }
            finally
            {
                SafeDelete(tempInput);
                SafeDelete(tempOutput);
            }
        }

        private static string ExtractFieldFromProfile(string profile, string fieldMarker)
        {
            if (string.IsNullOrWhiteSpace(profile))
                return string.Empty;

            var lines = profile.Split('\n');
            foreach (var line in lines)
            {
                if (line.Trim().StartsWith(fieldMarker, StringComparison.OrdinalIgnoreCase))
                {
                    var index = line.IndexOf(fieldMarker, StringComparison.OrdinalIgnoreCase);
                    return line.Substring(index + fieldMarker.Length).Trim();
                }
            }
            return string.Empty;
        }

        private static string ExtractJsonFromOutput(string output)
        {
            if (string.IsNullOrWhiteSpace(output))
                return null;

            // Find first { and last }
            var firstBrace = output.IndexOf('{');
            var lastBrace = output.LastIndexOf('}');

            if (firstBrace >= 0 && lastBrace > firstBrace)
            {
                return output.Substring(firstBrace, lastBrace - firstBrace + 1);
            }

            return null;
        }

        private static string ResolveDefaultToolingRoot()
        {
            // bin/Debug/net10.0 -> ../../.. = api-hrms/api-hrms -> ../.. = Multi-Repo
            var baseDir = AppContext.BaseDirectory;
            var candidate = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", "..", "arae-tooling"));
            return candidate;
        }

        private string TryReadModelVersion()
        {
            try
            {
                var metaPath = Path.Combine(_toolingRoot, "models", "model_metadata.json");
                if (File.Exists(metaPath))
                {
                    using var doc = JsonDocument.Parse(File.ReadAllText(metaPath));
                    if (doc.RootElement.TryGetProperty("model_version", out var versionEl))
                        return versionEl.GetString() ?? DefaultModelVersion;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to read ARAE model metadata; falling back to default version.");
            }

            return DefaultModelVersion;
        }

        private static void SafeDelete(string path)
        {
            try
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
            catch
            {
                // best-effort cleanup
            }
        }
    }
}