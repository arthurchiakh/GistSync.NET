using System.ComponentModel;
using System.Linq;
using GistSync.Core.Models;
using Microsoft.Extensions.Configuration;

namespace GistSync.Core.Utils
{
    public class DatabaseConfigurationProvider : ConfigurationProvider
    {
        private readonly GistSyncDbContext _dbContext;

        public DatabaseConfigurationProvider(GistSyncDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override void Load()
        {
            Data = _dbContext.Settings.ToDictionary(s => s.Key, s => s.Value);
        }

        public override void Set(string key, string value)
        {
            base.Set(key, value);

            // Persist back to db
            var result = _dbContext.Settings.FirstOrDefault(s => s.Key.Equals(key));

            if (result is null)
                _dbContext.Settings.Add(new Setting { Id = 0, Key = key, Value = value });
            else
                result.Value = value;

            _dbContext.SaveChanges();
        }
    }

    public class DatabaseConfigurationSource : IConfigurationSource
    {
        private readonly GistSyncDbContext _dbContext;

        public DatabaseConfigurationSource(GistSyncDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new DatabaseConfigurationProvider(_dbContext);
        }
    }

    public static class ConfigurationExtensions
    {
        public static T GetOrSet<T>(this IConfiguration config, string key, T defaultValue)
        {
            var stringValue = config[key];
            var converter = TypeDescriptor.GetConverter(typeof(T));

            if (stringValue is not null) return (T)converter.ConvertFromInvariantString(stringValue)!;

            var defaultValueString = converter.ConvertToString(defaultValue);
            config[key] = defaultValueString;
            return defaultValue;
        }

        public static void Set<T>(this IConfiguration config, string key, T defaultValue)
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));
            var defaultValueString = converter.ConvertToString(defaultValue);
            config[key] = defaultValueString;
        }
    }
}
