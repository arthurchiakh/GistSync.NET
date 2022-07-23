using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GistSync.Core.Models;
using GistSync.Core.Strategies;
using GistSync.Core.Strategies.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace GistSync.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void RegisterSyncStrategyProvider(this IServiceCollection serviceCollection)
        {
            var strategyTypes = new Dictionary<SyncModeTypes, Type>();

            var syncStrategyClassTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsClass && typeof(ISyncStrategy).IsAssignableFrom(t));

            foreach (var syncStrategy in syncStrategyClassTypes)
            {
                var associatedSyncTypeAttribute = Attribute.GetCustomAttribute(syncStrategy, typeof(RegisterForSyncStrategyAttribute));

                if (associatedSyncTypeAttribute == null)
                    throw new CustomAttributeFormatException(
                        $"The derived ISyncStrategy type must implement RegisterForSyncStrategyAttribute: {syncStrategy.AssemblyQualifiedName}");

                var associatedSyncType = (RegisterForSyncStrategyAttribute)associatedSyncTypeAttribute;

                if (strategyTypes.ContainsKey(associatedSyncType.SyncModeType))
                    throw new InvalidOperationException($"Multiple implementations for sync type {associatedSyncType}.");

                // Add to sync strategy type mapping
                strategyTypes.Add(associatedSyncType.SyncModeType, syncStrategy);

                // Register to service collection
                serviceCollection.AddSingleton(syncStrategy);
            }

            serviceCollection.AddSingleton(provider => new SyncStrategyProvider(provider, strategyTypes));
        }
    }
}
