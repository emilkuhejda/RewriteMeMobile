using System;
using System.Threading.Tasks;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Interfaces.Configuration;
using RewriteMe.Domain.Interfaces.Services;

namespace RewriteMe.Business.Configuration
{
    public class ApplicationSettings : IApplicationSettings
    {
        private const string Tenant = "rewriteme.onmicrosoft.com";
        private static readonly string AuthorityBase = $"https://login.microsoftonline.com/tfp/{Tenant}/";

        private readonly IInternalValueService _internalValueService;

        public ApplicationSettings(IInternalValueService internalValueService)
        {
            _internalValueService = internalValueService;
        }

#if DEBUG
        public Uri WebApiUri { get; } = new Uri("https://192.168.0.107:45456/");
        //public Uri WebApiUri { get; } = new Uri("https://rewrite-me.com/");
#else
        public Uri WebApiUri { get; } = new Uri("https://rewrite-me.com/");
#endif

        public Guid ApplicationId { get; private set; }

        public string SupportMailAddress => "emil.kuhejda@gmail.com";

        public string DefaultLanguage { get; } = "en";

        public string ClientId => "94983a85-6f54-4940-849e-55eaeb1d89dd";

        public string RedirectUri => "msal94983a85-6f54-4940-849e-55eaeb1d89dd://auth";

        public string[] Scopes => new[] { "" };

        public string PolicySignUpSignIn => "B2C_1_RewriteMe_SignUp_SignIn";

        public string PolicySignIn => "B2C_1_RewriteMe_SignIn";

        public string PolicyEditProfile => "B2C_1_RewriteMe_Edit";

        public string PolicyResetPassword => "B2C_1_RewriteMe_Password_Reset";

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
