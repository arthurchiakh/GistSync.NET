using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
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
            httpClient.DefaultRequestHeaders.UserAgent.Add(ProductInfoHeaderValue.Parse(AppConstants.AppName));
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));

            if (!string.IsNullOrWhiteSpace(personalAccessToken))
                httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"token {personalAccessToken}");

            return httpClient;
        }

        private JsonSerializerOptions DefaultJsonSerializerOptions => new() { PropertyNameCaseInsensitive = true };

        public async Task<Gist[]> Gists(string personalAccessToken, CancellationToken ct = default)
        {
            var httpClient = ConstructHttpClient(personalAccessToken);
            var response = await httpClient.GetAsync("gists", ct);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<Gist[]>(DefaultJsonSerializerOptions, ct);
        }

        public async Task<Gist> Gist(string gistId, string personalAccessToken = null!, CancellationToken ct = default)
        {
            var httpClient = ConstructHttpClient(personalAccessToken);
            var response = await httpClient.GetAsync($"gists/{gistId}", ct);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<Gist>(DefaultJsonSerializerOptions, ct);
        }

        public async Task<string> GetFileContentByUrl(string url, string personalAccessToken = null, CancellationToken ct = default)
        {
            var httpClient = ConstructHttpClient(personalAccessToken);
            var response = await httpClient.GetAsync(url, ct);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync(ct);
        }

        public async Task<Gist> PatchGist(string gistId, GistPatch gistPatch, string personalAccessToken = null, CancellationToken ct = default)
        {
            var httpClient = ConstructHttpClient(personalAccessToken);
            var response = await httpClient.PatchAsync($"gists/{gistId}", 
                new StringContent(JsonSerializer.Serialize(gistPatch), Encoding.UTF8),
                ct);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<Gist>(DefaultJsonSerializerOptions, ct);
        }

        public bool TryParseGistIdFromText(string text, out string gistId)
        {
            var regex = new Regex(@"(?:(?:(?:http|https):\/\/)?gist.github.com\/(?:.+?)/)?([0-9a-fA-F]{32})");
            var match = regex.Match(text);

            if (match.Success && !string.IsNullOrWhiteSpace(match.Groups[1].Value))
            {
                gistId = match.Groups[1].Value;

                return true;
            }

            gistId = string.Empty;
            return false;
        }
    }
}