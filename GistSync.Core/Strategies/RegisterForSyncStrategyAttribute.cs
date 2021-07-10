﻿using System;
using GistSync.Core.Models;

namespace GistSync.Core.Strategies
{
    /// <summary>
    /// Specify the type of sync strategy the class will be registered for.
    /// Only one class can be registered for each strategy types
    /// Else exception will be thrown during runtime
    /// </summary>
    [AttributeUsage(System.AttributeTargets.Class)]
    public class RegisterForSyncStrategyAttribute : Attribute
    {
        public SyncStrategyTypes SyncStrategyType { get; set; }

        public RegisterForSyncStrategyAttribute(SyncStrategyTypes syncStrategyType)
        {
            SyncStrategyType = syncStrategyType;
        }
    }
}
