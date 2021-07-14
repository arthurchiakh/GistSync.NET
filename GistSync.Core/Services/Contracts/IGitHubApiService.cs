using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using GistSync.Core.Models.GitHub;

namespace GistSync.Core.Services.Contracts
{
    public interface IGitHubApiService
    {
        /// <summary>
        /// Lists the authenticated user's gists or if called anonymously, this endpoint returns all public gists
        /// </summary>
        /// <param name="personalAccessToken">Personal Access Token</param>
        /// <param name="ct">Cancellation Token</param>
        /// <returns>Array of gists</returns>
        Task<Gist[]> Gists(string personalAccessToken, CancellationToken ct = default);

        /// <summary>
        /// Get a gist by gist Id
        /// </summary>
        /// <param name="gistId">Gist Id</param>
        /// <param name="personalAccessToken">Personal Access Token</param>
        /// <param name="ct">Cancellation Token</param>
        /// <returns>Gist</returns>
        Task<Gist> Gist([NotNull] string gistId, string personalAccessToken = null, CancellationToken ct = default);

        Task<string> GetFileContentByUrl(string url, string personalAccessToken = null, CancellationToken ct = default);
        Task<Gist> PatchGist(string gistId, GistPatch gistPatch, string personalAccessToken = null, CancellationToken ct = default);
    }
}
