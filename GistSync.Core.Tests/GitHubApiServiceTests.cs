using System.Threading.Tasks;
using GistSync.Core.Services;
using GistSync.Core.Tests.Utils;
using NUnit.Framework;

namespace GistSync.Core.Tests
{
    public class GitHubApiServiceTests
    {
        private GitHubApiService _gitHubApiService;
        private string _personalAccessToken;

        [SetUp]
        public void TestSetUp()
        {
            _personalAccessToken = UserSecretUtil.Configuration["GitHub:PersonalAccessToken"];
            if (string.IsNullOrWhiteSpace(_personalAccessToken))
                Assert.Ignore("Test ignored due to GitHub Personal Access Token not provided in user secret.");
            else
                _gitHubApiService = new GitHubApiService();
        }

        [Test]
        public async Task GitHubApiService_Gists()
        {
            var gists = await _gitHubApiService.Gists(_personalAccessToken);
            Assert.NotNull(gists);
        }

        [Test]
        public async Task GitHubApiService_Gist()
        {
            var gists = await _gitHubApiService.Gist("db136774b432d328fcc041ea0ea1f88a", _personalAccessToken);
            Assert.NotNull(gists);
        }
    }
}