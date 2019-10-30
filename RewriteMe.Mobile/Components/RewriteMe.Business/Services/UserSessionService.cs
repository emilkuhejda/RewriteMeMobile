using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Push;
using Microsoft.Identity.Client;
using Plugin.SecureStorage;
using RewriteMe.Business.Wrappers;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Enums;
using RewriteMe.Domain.Exceptions;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.Interfaces.Configuration;
using RewriteMe.Domain.Interfaces.Factories;
using RewriteMe.Domain.Interfaces.Repositories;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Messages;
using RewriteMe.Domain.WebApi.Models;
using RewriteMe.Logging.Extensions;
using RewriteMe.Logging.Interfaces;
using Xamarin.Forms;
using FormsDevice = Xamarin.Forms.Device;

namespace RewriteMe.Business.Services
{
    public class UserSessionService : IUserSessionService
    {
        private const string AccessTokenKey = "AccessToken";

        private readonly ILanguageService _languageService;
        private readonly IRegistrationUserWebService _registrationUserWebService;
        private readonly ICleanUpService _cleanUpService;
        private readonly IAppCenterMetricsService _appCenterMetricsService;
        private readonly IPublicClientApplication _publicClientApplication;
        private readonly IIdentityUiParentProvider _identityUiParentProvider;
        private readonly IApplicationVersionProvider _applicationVersionProvider;
        private readonly IApplicationSettings _applicationSettings;
        private readonly IUserSessionRepository _userSessionRepository;
        private readonly IUserSubscriptionRepository _userSubscriptionRepository;
        private readonly ILogger _logger;
        private readonly object _lockObject = new object();

        private Guid _userId = Guid.Empty;
        private AccessToken _accessToken;

        public UserSessionService(
            ILanguageService languageService,
            IRegistrationUserWebService registrationUserWebService,
            ICleanUpService cleanUpService,
            IAppCenterMetricsService appCenterMetricsService,
            IPublicClientApplicationFactory publicClientApplicationFactory,
            IIdentityUiParentProvider identityUiParentProvider,
            IApplicationVersionProvider applicationVersionProvider,
            IApplicationSettings applicationSettings,
            IUserSessionRepository userSessionRepository,
            IUserSubscriptionRepository userSubscriptionRepository,
            ILoggerFactory loggerFactory)
        {
            _languageService = languageService;
            _registrationUserWebService = registrationUserWebService;
            _cleanUpService = cleanUpService;
            _appCenterMetricsService = appCenterMetricsService;
            _identityUiParentProvider = identityUiParentProvider;
            _applicationVersionProvider = applicationVersionProvider;
            _applicationSettings = applicationSettings;
            _userSessionRepository = userSessionRepository;
            _userSubscriptionRepository = userSubscriptionRepository;
            _logger = loggerFactory.CreateLogger(typeof(UserSessionService));

            _publicClientApplication = publicClientApplicationFactory.CreatePublicClientApplication(
                applicationSettings.ClientId,
                applicationSettings.RedirectUri);
        }

        public AccessToken AccessToken => _accessToken ?? (_accessToken = new AccessToken(GetToken()));

        public async Task<Guid> GetUserIdAsync()
        {
            if (_userId != Guid.Empty)
                return _userId;

            var userSession = await _userSessionRepository.GetUserSessionAsync().ConfigureAwait(false);
            ValidateUserSession(userSession);

            return _userId = userSession.ObjectId;
        }

        public async Task<string> GetUserNameAsync()
        {
            var userSession = await _userSessionRepository.GetUserSessionAsync().ConfigureAwait(false);
            ValidateUserSession(userSession);

            return $"{userSession.GivenName} {userSession.FamilyName}";
        }

        public async Task<UserSession> GetUserSessionAsync()
        {
            return await _userSessionRepository.GetUserSessionAsync().ConfigureAwait(false);
        }

        public string GetToken()
        {
            lock (_lockObject)
            {
                if (!CrossSecureStorage.Current.HasKey(AccessTokenKey))
                    return null;

                return CrossSecureStorage.Current.GetValue(AccessTokenKey);
            }
        }

        public void SetToken(string accessToken)
        {
            _accessToken = null;

            CrossSecureStorage.Current.SetValue(AccessTokenKey, accessToken);
        }

        public async Task<bool> IsSignedInAsync()
        {
            var token = GetToken();
            if (token == null)
                return false;

            var userSessionExists = await _userSessionRepository.UserSessionExistsAsync().ConfigureAwait(false);
            if (!userSessionExists)
                return false;

            return true;
        }

        public async Task<B2CAccessToken> SignUpOrInAsync()
        {
            return await SignUpOrInAsync(_applicationSettings.PolicySignUpSignIn, _applicationSettings.AuthoritySignUpSignIn).ConfigureAwait(false);
        }

        private async Task<B2CAccessToken> SignUpOrInAsync(string policy, string authority)
        {
            _logger.Info($"Sign-in (with policy '{policy}')");

            await RemoveLocalAccountsAsync().ConfigureAwait(false);

            var uiParent = _identityUiParentProvider.GetUiParent();
            var accounts = await GetAccountsLocalAsync().ConfigureAwait(false);

            var user = GetUserByPolicy(accounts, policy);
            try
            {
                var signUpOrInResult = await _publicClientApplication
                    .AcquireTokenInteractive(_applicationSettings.Scopes)
                    .WithAccount(user)
                    .WithAuthority(authority)
                    .WithParentActivityOrWindow(uiParent)
                    .WithUseEmbeddedWebView(true)
                    .ExecuteAsync()
                    .ConfigureAwait(false);

                if (signUpOrInResult.IdToken == null)
                    return null;

                var accessToken = new B2CAccessToken(signUpOrInResult.IdToken);

                await UpdateUserSessionAsync(accessToken).ConfigureAwait(false);
                return accessToken;
            }
            catch (HttpRequestException)
            {
                // Application is currently offline
                _logger.Warning("No connection could be established to the authority.");
            }
            catch (MsalException e)
            {
                if (e.Message.Contains("AADB2C90118"))
                {
                    // Password reset requested
                    _logger.Info("The user has requested to reset the password.");
                    return await ResetPasswordAsync().ConfigureAwait(false);
                }
                else if (e.Message.Contains("AADB2C90091"))
                {
                    _logger.Info("The user has canceled the sign-up.");
                }
                else if ("authentication_canceled".Equals(e.ErrorCode, StringComparison.Ordinal))
                {
                    _logger.Info("The user has canceled the sign-in.");
                }
                else
                {
                    _logger.Exception(e);
                }
            }

            return null;
        }

        public async Task<B2CAccessToken> EditProfileAsync()
        {
            _logger.Info("Edit profile");

            var uiParent = _identityUiParentProvider.GetUiParent();
            var accounts = await GetAccountsLocalAsync().ConfigureAwait(false);

            var user = GetUserByPolicy(accounts, _applicationSettings.PolicyEditProfile);
            try
            {
                var result = await _publicClientApplication
                    .AcquireTokenInteractive(_applicationSettings.Scopes)
                    .WithAccount(user)
                    .WithAuthority(_applicationSettings.AuthorityEditProfile)
                    .WithParentActivityOrWindow(uiParent)
                    .WithUseEmbeddedWebView(true)
                    .ExecuteAsync()
                    .ConfigureAwait(false);

                if (result.IdToken == null)
                    return null;

                var accessToken = new B2CAccessToken(result.IdToken);

                await UpdateUserSessionAsync(accessToken).ConfigureAwait(false);
                await UpdateUserAsync(accessToken).ConfigureAwait(false);

                return accessToken;
            }
            catch (Exception ex)
            {
                _logger.Error(ExceptionFormatter.FormatException(ex));
                _appCenterMetricsService.TrackException(ex);
            }

            return null;
        }

        public async Task<B2CAccessToken> ResetPasswordAsync()
        {
            _logger.Info("Reset password");

            var uiParent = _identityUiParentProvider.GetUiParent();
            var accounts = await GetAccountsLocalAsync().ConfigureAwait(false);

            var user = GetUserByPolicy(accounts, _applicationSettings.AuthorityPasswordReset);

            try
            {
                var result = await _publicClientApplication
                    .AcquireTokenInteractive(_applicationSettings.Scopes)
                    .WithAccount(user)
                    .WithAuthority(_applicationSettings.AuthorityPasswordReset)
                    .WithParentActivityOrWindow(uiParent)
                    .WithUseEmbeddedWebView(true)
                    .ExecuteAsync()
                    .ConfigureAwait(false);

                if (result.IdToken == null)
                    return null;

                var accessToken = new B2CAccessToken(result.IdToken);
                await UpdateUserSessionAsync(accessToken).ConfigureAwait(false);

                return accessToken;
            }
            catch (Exception ex)
            {
                _logger.Error(ExceptionFormatter.FormatException(ex));
                _appCenterMetricsService.TrackException(ex);
            }

            return null;
        }

        public async Task SignOutAsync()
        {
            _logger.Info("Sign out");

            _userId = Guid.Empty;
            _accessToken = null;

            NotifyBackgroundServices();

            await RemoveLocalAccountsAsync().ConfigureAwait(false);
            await _cleanUpService.CleanUp().ConfigureAwait(false);
            await Push.SetEnabledAsync(false).ConfigureAwait(false);

            CrossSecureStorage.Current.DeleteKey(AccessTokenKey);
        }

        public void NotifyBackgroundServices()
        {
            MessagingCenter.Send(new StopBackgroundServiceMessage(BackgroundServiceType.TranscribeItem), nameof(BackgroundServiceType.TranscribeItem));
            MessagingCenter.Send(new StopBackgroundServiceMessage(BackgroundServiceType.Synchronizer), nameof(BackgroundServiceType.Synchronizer));
        }

        private async Task RemoveLocalAccountsAsync()
        {
            var accounts = await GetAccountsLocalAsync().ConfigureAwait(false);

            foreach (var user in accounts)
            {
                await _publicClientApplication.RemoveAsync(user).ConfigureAwait(false);
            }
        }

        private async Task<IEnumerable<IAccount>> GetAccountsLocalAsync()
        {
            IEnumerable<IAccount> accounts;

            try
            {
                accounts = await _publicClientApplication.GetAccountsAsync().ConfigureAwait(false);
            }
            catch (HttpRequestException)
            {
                accounts = new List<IAccount>();
            }

            return accounts;
        }

        private IAccount GetUserByPolicy(IEnumerable<IAccount> users, string policy)
        {
            try
            {
                foreach (var user in users)
                {
                    var identifiers = user.HomeAccountId.Identifier.Split('.');
                    var userIdentifier = identifiers[0];
                    if (userIdentifier.EndsWith(policy.ToLowerInvariant(), StringComparison.Ordinal))
                    {
                        return EmptyUsernameAccountWrapper.WrapOrNull(user);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Exception(ex, "Failed to get userIdentifier");
                _appCenterMetricsService.TrackException(ex);
            }

            return null;
        }

        private async Task UpdateUserSessionAsync(B2CAccessToken accessToken)
        {
            if (accessToken == null)
                throw new ArgumentNullException(nameof(accessToken));

            _logger.Debug($"Update user session for '{accessToken.GivenName} {accessToken.FamilyName}' with id '{accessToken.ObjectId}'.");

            var userSession = await _userSessionRepository.GetUserSessionAsync().ConfigureAwait(false);
            userSession.ObjectId = Guid.Parse(accessToken.ObjectId);
            userSession.Email = accessToken.Email;
            userSession.GivenName = accessToken.GivenName;
            userSession.FamilyName = accessToken.FamilyName;

            ValidateUserSession(userSession);

            await _userSessionRepository.UpdateUserSessionAsync(userSession).ConfigureAwait(false);
        }

        public async Task RegisterUserAsync(B2CAccessToken accessToken)
        {
            if (accessToken == null)
                throw new ArgumentNullException(nameof(accessToken));

            var installationId = await AppCenter.GetInstallIdAsync().ConfigureAwait(false) ?? Guid.Empty;
            var language = await _languageService.GetLanguageName().ConfigureAwait(false);
            var registrationDeviceModel = new RegistrationDeviceModel
            {
                InstallationId = installationId,
                RuntimePlatform = FormsDevice.RuntimePlatform,
                InstalledVersionNumber = _applicationVersionProvider.GetInstalledVersionNumber(),
                Language = language
            };

            var registerUserModel = new RegistrationUserModel
            {
                Id = Guid.Parse(accessToken.ObjectId),
                ApplicationId = _applicationSettings.ApplicationId,
                Email = accessToken.Email,
                GivenName = accessToken.GivenName,
                FamilyName = accessToken.FamilyName,
                Device = registrationDeviceModel
            };

            var httpRequestResult = await _registrationUserWebService.RegisterUserAsync(registerUserModel, accessToken.Token).ConfigureAwait(false);
            if (httpRequestResult.State != HttpRequestState.Success)
                throw new UserRegistrationFailedException();

            SetToken(httpRequestResult.Payload.Token);

            await _userSubscriptionRepository.AddAsync(httpRequestResult.Payload.UserSubscription).ConfigureAwait(false);
        }

        private async Task UpdateUserAsync(B2CAccessToken accessToken)
        {
            if (accessToken == null)
                throw new ArgumentNullException(nameof(accessToken));

            var updateUserModel = new UpdateUserModel
            {
                GivenName = accessToken.GivenName,
                FamilyName = accessToken.FamilyName
            };

            await _registrationUserWebService.UpdateUserAsync(updateUserModel, GetToken()).ConfigureAwait(false);
        }

        private void ValidateUserSession(UserSession userSession)
        {
            if (userSession.ObjectId == Guid.Empty)
            {
                throw new InvalidOperationException("The current user session is not valid.");
            }
        }
    }
}
