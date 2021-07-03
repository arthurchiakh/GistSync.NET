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
        /// <returns>Array of gists</returns>
        Task<Gist[]> Gists(string personalAccessToken);

        /// <summary>
        /// Lists the authenticated user's gists or if called anonymously, this endpoint returns all public gists
        /// </summary>
        /// <param name="personalAccessToken">Personal Access Token</param>
        /// <param name="ct">Cancellation Token</param>
        /// <returns>Array of gists</returns>
        Task<Gist[]> Gists(string personalAccessToken, CancellationToken ct);

        /// <summary>
        /// Get a gist by gist id
        /// </summary>
        /// <param name="gistId">Gist Id</param>
        /// <param name="personalAccessToken">Personal Access Token</param>
        /// <returns>Gist</returns>
        Task<Gist> Gist([NotNull] string gistId, string personalAccessToken = null);

        /// <summary>
        /// Get a gist by gist id
        /// </summary>
        /// <param name="gistId">Gist Id</param>
        /// <param name="personalAccessToken">Personal Access Token</param>
        /// <param name="ct">Cancellation Token</param>
        /// <returns>Gist</returns>
        Task<Gist> Gist([NotNull] string gistId, string personalAccessToken, CancellationToken ct);
    }
}
