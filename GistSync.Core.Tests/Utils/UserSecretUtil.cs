using Microsoft.Extensions.Configuration;

namespace GistSync.Core.Tests.Utils
{
    public class UserSecretUtil
    {
        public static IConfiguration Configuration;

        static UserSecretUtil()
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddUserSecrets(typeof(UserSecretUtil).Assembly);
            Configuration = configurationBuilder.Build();
        }
    }
}