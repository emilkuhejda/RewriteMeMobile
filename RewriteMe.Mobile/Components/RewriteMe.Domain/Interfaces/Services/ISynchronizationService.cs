﻿using System;
using System.Threading.Tasks;
using RewriteMe.Domain.Events;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface ISynchronizationService
    {
        event EventHandler<ProgressEventArgs> InitializationProgress;

        Task InitializeAsync();
    }
}
