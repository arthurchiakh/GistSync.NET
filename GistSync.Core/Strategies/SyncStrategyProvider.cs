using System;
using System.Collections.Generic;
using GistSync.Core.Models;
using GistSync.Core.Strategies.Contracts;

namespace GistSync.Core.Strategies
{
    public class SyncStrategyProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDictionary<SyncStrategyTypes, Type> _strategyTypes;

        public SyncStrategyProvider(IServiceProvider serviceProvider, IDictionary<SyncStrategyTypes, Type> strategyTypes)
        {
            _serviceProvider = serviceProvider;
            _strategyTypes = strategyTypes;
        }

        public ISyncStrategy Provide(SyncStrategyTypes syncStrategyType)
        {
            if (!_strategyTypes.TryGetValue(syncStrategyType, out var classType))
                throw new InvalidOperationException($"There's no sync strategy register for sync type [{syncStrategyType.GetType()}.{syncStrategyType}]");

            return (ISyncStrategy)_serviceProvider.GetService(classType);
        }
    }
}
