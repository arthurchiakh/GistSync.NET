using System;

namespace GistSync.Core.Models.Settings
{
    public class GitHub
    {
        public int StatusRefreshIntervalSeconds { get; set; } = TimeSpan.FromMinutes(5).Seconds;
        public int MaxConcurrentGistStatusCheck { get; set; } = 5;
    }
}