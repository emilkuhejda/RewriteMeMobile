﻿using System;
using System.Threading.Tasks;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface ILastUpdatesService
    {
        bool IsConnectionSuccessful { get; }

        Task InitializeAsync();

        Task InitializeApplicationSettingsAsync();

        DateTime GetFileItemLastUpdate();

        DateTime GetDeletedFileItemLastUpdate();

        DateTime GetTranscribeItemLastUpdate();

        DateTime GetUserSubscriptionLastUpdate();

        DateTime GetInformationMessageLastUpdate();
    }
}
