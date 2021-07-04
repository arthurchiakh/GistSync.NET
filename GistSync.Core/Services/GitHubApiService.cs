using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GistSync.Core.Models.GitHub;
using GistSync.Core.Services.Contracts;

namespace GistSync.Core.Services
{ 
    public class GitHubApiService : IGitHubApiService
    {
        public const string GITHUB_API_URL = "https://api.github.com/";

        private HttpClient ConstructHttpClient(string personalAccessToken = null)
        {
            var httpClient = new HttpClient { BaseAddress = new Uri(GITHUB_API_URL) };
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.UserAgent.Add(ProductInfoHeaderValue.Parse(Constants.AppName));
            if (!string.IsNullOrWhiteSpace(personalAccessToken))
                httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"token {personalAccessToken}");

            return httpClient;
        }

        private JsonSerializerOptions DefaultJsonSerializerOptions => new() { PropertyNameCaseInsensitive = true };

        public Task<Gist[]> Gists(string personalAccessToken = null)
        {
            return Gists(personalAccessToken, CancellationToken.None);
        }

        public async Task<Gist[]> Gists(string personalAccessToken, CancellationToken ct)
        {
            var httpClient = ConstructHttpClient(personalAccessToken);
            var response = await httpClient.GetAsync("gists", ct);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<Gist[]>(DefaultJsonSerializerOptions, ct);
        }

        public Task<Gist> Gist([NotNull] string gistId, string personalAccessToken = null)
        {
            return Gist(gistId, personalAccessToken, CancellationToken.None);
        }

        public async Task<Gist> Gist([NotNull] string gistId, string personalAccessToken, CancellationToken ct)
        {
            var httpClient = ConstructHttpClient(personalAccessToken);
            var response = await httpClient.GetAsync($"gists/{gistId}", ct);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<Gist>(DefaultJsonSerializerOptions, ct);
        }
    }
}