using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace GistSync.Core.Tests.Utils
{
    public static class ConfigurationUtil
    {
        public static IConfigurationRoot MockConfiguration(Dictionary<string, string> inMemorySettings = null)
        {
            return new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
        }
    }
}