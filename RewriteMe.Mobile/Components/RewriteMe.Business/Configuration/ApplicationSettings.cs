using System;
using System.Threading.Tasks;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Interfaces.Configuration;
using RewriteMe.Domain.Interfaces.Services;

namespace RewriteMe.Business.Configuration
{
    public class ApplicationSettings : IApplicationSettings
    {
        private const string HostUrl = "https://voicipher.com/";
        private const string OsxAppCenterKey = "38ccf01c-fc36-4b94-a5c2-743c02852b23";
        private const string AndroidAppCenterKey = "a888ddaf-c5e2-4461-ae7e-0d896cf022df";
        private const string Tenant = "voicipher.onmicrosoft.com";
        private static readonly string AuthorityBase = $"https://login.microsoftonline.com/tfp/{Tenant}/";

        private readonly IInternalValueService _internalValueService;

        public ApplicationSettings(IInternalValueService internalValueService)
        {
            _internalValueService = internalValueService;
        }

        public Guid ApplicationId { get; private set; }

        public string WebApiUrl { get; } = HostUrl;

        public Uri PrivacyPolicyUri { get; } = new Uri($"{HostUrl}home/privacy/");

        public string CacheHubUrl { get; } = $"{HostUrl}api/message-hub/";

        public string WebApiVersion { get; } = "1";

        public string AppCenterKeys { get; } = $"ios={OsxAppCenterKey};android={AndroidAppCenterKey}";

        public string SyncfusionKey { get; } = "MjQ2NDkwQDMxMzgyZTMxMmUzMGlRdnQ1Z1VVSTltakN0c2VBZmdEQ2RReFQ0SEdoVThEMzYxSDZIdnlYWWM9";

        public string SupportMailAddress => "support@voicipher.com";

        public string DefaultLanguage { get; } = "en";

        public string ClientId => "3f16cd47-52fe-44e6-96b3-131e1e57b09c";

        public string RedirectUri => "msal3f16cd47-52fe-44e6-96b3-131e1e57b09c://auth";

        public string[] Scopes => new[] { "" };

        public string PolicySignUpSignIn => "B2C_1_Voicipher_SignUp_SignIn";

        public string PolicySignIn => "B2C_1_Voicipher_SignIn";

        public string PolicyEditProfile => "B2C_1_Voicipher_Edit";

        public string PolicyResetPassword => "B2C_1_Voicipher_Password_Reset";

        public string AuthoritySignUpSignIn => $"{AuthorityBase}{PolicySignUpSignIn}";

        public string AuthoritySignIn => $"{AuthorityBase}{PolicySignIn}";

        public string AuthorityEditProfile => $"{AuthorityBase}{PolicyEditProfile}";

        public string AuthorityPasswordReset => $"{AuthorityBase}{PolicyResetPassword}";

        public async Task InitializeAsync()
        {
            var applicationId = await _internalValueService.GetValueAsync(InternalValues.ApplicationId).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(applicationId))
            {
                applicationId = Guid.NewGuid().ToString();

                await _internalValueService.UpdateValueAsync(InternalValues.ApplicationId, applicationId).ConfigureAwait(false);
            }

            ApplicationId = Guid.Parse(applicationId);
        }
    }
}
