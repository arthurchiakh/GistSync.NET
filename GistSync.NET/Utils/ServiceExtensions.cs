using Microsoft.Extensions.DependencyInjection;

namespace GistSync.NET.Utils
{
    public static class ServiceExtensions
    {
        public static void AddForm<T>(this IServiceCollection serviceCollection) where T : Form
        {
            serviceCollection.AddTransient<T>();
        }

        public static T GetForm<T>(this IServiceProvider serviceProvider) where T : Form
        {
            return serviceProvider.GetRequiredService<T>();
        }
    }
}
