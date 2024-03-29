﻿using System;
using System.Threading.Tasks;

namespace RewriteMe.Domain.Interfaces.Configuration
{
    public interface IApplicationSettings
    {
        Guid ApplicationId { get; }

        string WebApiUrl { get; }

        Uri PrivacyPolicyUri { get; }

        string HubUrl { get; }

        string WebApiVersion { get; }

        string AppCenterKeys { get; }

        string SyncfusionKey { get; }

        string SupportMailAddress { get; }

        string DefaultLanguage { get; }

        string ClientId { get; }

        string RedirectUri { get; }

        string[] Scopes { get; }

        string PolicySignUpSignIn { get; }

        string PolicySignIn { get; }

        string PolicyEditProfile { get; }

        string PolicyResetPassword { get; }

        string AuthoritySignUpSignIn { get; }

        string AuthoritySignIn { get; }

        string AuthorityEditProfile { get; }

        string AuthorityPasswordReset { get; }

        Task InitializeAsync();
    }
}
