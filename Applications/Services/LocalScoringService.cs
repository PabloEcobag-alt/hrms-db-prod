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
                var payload = new
                {
                    position = position ?? string.Empty,
                    contact_details = applicantProfile ?? string.Empty,
                    source = "Unknown"
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
                var result = JsonSerializer.Deserialize<ApplicantScore>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? throw new InvalidOperationException("Local scorer returned an empty or invalid score.");

                result.MatchScore = Math.Clamp(result.MatchScore, 0, 100);
                return result;
            }
            finally
            {
                SafeDelete(tempInput);
                SafeDelete(tempOutput);
            }
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
