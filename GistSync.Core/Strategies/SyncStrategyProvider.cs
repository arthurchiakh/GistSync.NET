using System;
using System.Collections.Generic;
using GistSync.Core.Models;
using GistSync.Core.Strategies.Contracts;

namespace GistSync.Core.Strategies
{
    public class SyncStrategyProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDictionary<SyncModeTypes, Type> _strategyTypes;

        public SyncStrategyProvider(IServiceProvider serviceProvider, IDictionary<SyncModeTypes, Type> strategyTypes)
        {
            _serviceProvider = serviceProvider;
            _strategyTypes = strategyTypes;
        }

        public ISyncStrategy Provide(SyncModeTypes syncModeType)
        {
            if (!_strategyTypes.TryGetValue(syncModeType, out var classType))
                throw new InvalidOperationException($"There's no sync strategy register for sync type [{syncModeType.GetType()}.{syncModeType}]");

            return (ISyncStrategy)_serviceProvider.GetService(classType);
        }
    }
}
