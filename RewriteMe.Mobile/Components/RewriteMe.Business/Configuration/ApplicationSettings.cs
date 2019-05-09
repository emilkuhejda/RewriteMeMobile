using System;
using System.Threading.Tasks;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Interfaces.Configuration;
using RewriteMe.Domain.Interfaces.Services;

namespace RewriteMe.Business.Configuration
{
    public class ApplicationSettings : IApplicationSettings
    {
        private const string Tenant = "rewritemedemo.onmicrosoft.com";
        private static readonly string AuthorityBase = $"https://login.microsoftonline.com/tfp/{Tenant}/";

        private readonly IInternalValueService _internalValueService;

        public ApplicationSettings(IInternalValueService internalValueService)
        {
            _internalValueService = internalValueService;
        }

        public Uri WebApiUri { get; } = new Uri("http://rewriteme.aspifyhost.cz/");

        public Guid ApplicationId { get; private set; }

        public string SupportMailAddress => "emil.kuhejda@gmail.com";

        public string ClientId => "cbd1aee8-27d8-49f4-9f73-02337c15c7a3";

        public string RedirectUri => "msalcbd1aee8-27d8-49f4-9f73-02337c15c7a3://auth";

        public string[] Scopes => new[] { "" };

        public string PolicySignUpSignIn => "B2C_1_Demo_sign_up";

        public string PolicySignIn => "B2C_1_Demo_signup";

        public string PolicyEditProfile => "B2C_1_Demo_edit_profile";

        public string PolicyResetPassword => "B2C_1_Demo_reset_password";

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
