using System.Diagnostics;
using System.Text.Json;
using Applications.Interfaces;
using Microsoft.Extensions.Logging;

namespace Applications.Services
{
    /// <summary>
    /// Local vector search service that shells out to the Python ARAE tooling
    /// (arae-tooling/search_candidates.py). Queries ChromaDB for semantic
    /// candidate matching.
    /// </summary>
    public class LocalVectorSearchService : IVectorSearchService
    {
        private readonly ILogger<LocalVectorSearchService> _logger;
        private readonly string _toolingRoot;
        private readonly string _pythonExecutable;

        public LocalVectorSearchService(ILogger<LocalVectorSearchService> logger)
        {
            _logger = logger;

            _toolingRoot = Environment.GetEnvironmentVariable("ARAE_TOOLING_ROOT")
                ?? ResolveDefaultToolingRoot();

            _pythonExecutable = Environment.GetEnvironmentVariable("ARAE_PYTHON_EXECUTABLE")
                ?? "python3";
        }

        public async Task<List<CandidateSearchResult>> SearchCandidatesAsync(string query, CancellationToken cancellationToken = default)
        {
            var searchScript = Path.Combine(_toolingRoot, "search_candidates.py");
            if (!File.Exists(searchScript))
            {
                throw new FileNotFoundException(
                    $"ARAE search_candidates.py not found at '{searchScript}'. " +
                    "Set ARAE_TOOLING_ROOT or run from the expected repository layout.");
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = _pythonExecutable,
                Arguments = $"\"{searchScript}\" --query \"{query}\"",
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
                _logger.LogError("Vector search failed with exit code {ExitCode}. Stderr: {Stderr}",
                    process.ExitCode, stderr);
                throw new InvalidOperationException($"Vector search failed: {stderr}");
            }

            _logger.LogDebug("Vector search stdout: {Stdout}", stdout);

            if (string.IsNullOrWhiteSpace(stdout))
            {
                return new List<CandidateSearchResult>();
            }

            var results = JsonSerializer.Deserialize<List<CandidateSearchResult>>(stdout, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<CandidateSearchResult>();

            return results;
        }

        private static string ResolveDefaultToolingRoot()
        {
            // bin/Debug/net10.0 -> ../../.. = api-hrms/api-hrms -> ../.. = Multi-Repo
            var baseDir = AppContext.BaseDirectory;
            var candidate = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", "..", "arae-tooling"));
            return candidate;
        }
    }
}
