using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Applications.Interfaces;
using Api.Contracts.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ApiHrm.Infrastructures.Services
{
    public class AuthServiceClient : IAuthServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuthServiceClient> _logger;

        public AuthServiceClient(
            HttpClient httpClient,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AuthServiceClient> logger)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<string> ProvisionUserAsync(CreateUserRequest request)
        {
            _httpClient.DefaultRequestHeaders.Add("X-Internal-Token", "HrAppInternalTokenSecret");

            var response = await _httpClient.PostAsJsonAsync("https://localhost:5001/api/users/internal", request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to provision user in Auth Service. Status Code: {response.StatusCode}. Error: {errorContent}");
            }

            var content = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(content);
            
            if (document.RootElement.TryGetProperty("id", out var idElement))
            {
                return idElement.GetString() ?? throw new Exception("Auth service returned null ID.");
            }

            throw new Exception("ID not found in Auth Service response.");
        }
    }
}
