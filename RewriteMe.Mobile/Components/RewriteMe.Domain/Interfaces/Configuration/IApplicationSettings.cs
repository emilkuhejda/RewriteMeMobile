﻿using System;

namespace RewriteMe.Domain.Interfaces.Configuration
{
    public interface IApplicationSettings
    {
        Uri WebApiUri { get; }

        string SupportMailAddress { get; }

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
    }
}
