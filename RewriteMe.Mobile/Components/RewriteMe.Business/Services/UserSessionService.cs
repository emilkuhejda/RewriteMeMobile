using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Plugin.SecureStorage;
using RewriteMe.Business.Configuration;
using RewriteMe.Business.Wrappers;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Exceptions;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.Interfaces.Configuration;
using RewriteMe.Domain.Interfaces.Factories;
using RewriteMe.Domain.Interfaces.Repositories;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.WebApi.Models;
using RewriteMe.Logging.Extensions;
using RewriteMe.Logging.Interfaces;

namespace RewriteMe.Business.Services
{
    public class UserSessionService : IUserSessionService
    {
        private const string AccessTokenKey = "AccessToken";

        private readonly IRegistrationUserWebService _registrationUserWebService;
        private readonly ICleanUpService _cleanUpService;
        private readonly IPublicClientApplication _publicClientApplication;
        private readonly IIdentityUiParentProvider _identityUiParentProvider;
        private readonly IApplicationSettings _applicationSettings;
        private readonly IUserSessionRepository _userSessionRepository;
        private readonly IUserSubscriptionRepository _userSubscriptionRepository;
        private readonly ILogger _logger;

        private Guid _userId = Guid.Empty;

        public UserSessionService(
            IRegistrationUserWebService registrationUserWebService,
            ICleanUpService cleanUpService,
            IPublicClientApplicationFactory publicClientApplicationFactory,
            IIdentityUiParentProvider identityUiParentProvider,
            IApplicationSettings applicationSettings,
            IUserSessionRepository userSessionRepository,
            IUserSubscriptionRepository userSubscriptionRepository,
            ILoggerFactory loggerFactory)
        {
            _registrationUserWebService = registrationUserWebService;
            _cleanUpService = cleanUpService;
            _identityUiParentProvider = identityUiParentProvider;
            _applicationSettings = applicationSettings;
            _userSessionRepository = userSessionRepository;
            _userSubscriptionRepository = userSubscriptionRepository;
            _logger = loggerFactory.CreateLogger(typeof(UserSessionService));

            _publicClientApplication = publicClientApplicationFactory.CreatePublicClientApplication(
                applicationSettings.ClientId,
                applicationSettings.RedirectUri);
        }

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

        public string GetAccessToken()
        {
            return CrossSecureStorage.Current.GetValue(AccessTokenKey);
        }

        public void SetAccessToken(string accessToken)
        {
            CrossSecureStorage.Current.SetValue(AccessTokenKey, accessToken);
        }

        public async Task<bool> IsSignedInAsync()
        {
            var token = GetAccessToken();
            if (token == null)
                return false;

            var userSessionExists = await _userSessionRepository.UserSessionExistsAsync().ConfigureAwait(false);
            if (!userSessionExists)
                return false;

            return true;
        }

        public async Task<bool> SignUpOrInAsync()
        {
            return await SignUpOrInAsync(_applicationSettings.PolicySignUpSignIn, _applicationSettings.AuthoritySignUpSignIn).ConfigureAwait(false);
        }

        private async Task<bool> SignUpOrInAsync(string policy, string authority)
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
                    .ExecuteAsync()
                    .ConfigureAwait(false);

                await UpdateUserSessionAndRegisterUserAsync(signUpOrInResult.IdToken).ConfigureAwait(false);
                return signUpOrInResult.IdToken != null;
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
                    var passwordResetSuccessful = await ResetPasswordAsync().ConfigureAwait(false);
                    if (passwordResetSuccessful)
                    {
                        return true;
                    }
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

            return false;
        }

        public async Task<bool> EditProfileAsync()
        {
            _logger.Info("Edit profile");

            var uiParent = _identityUiParentProvider.GetUiParent();
            var accounts = await GetAccountsLocalAsync().ConfigureAwait(false);

            var user = GetUserByPolicy(accounts, _applicationSettings.PolicyEditProfile);
            try
            {
                var result = await _publicClientApplication.AcquireTokenInteractive(_applicationSettings.Scopes)
                    .WithAccount(user)
                    .WithAuthority(_applicationSettings.AuthorityEditProfile)
                    .WithParentActivityOrWindow(uiParent)
                    .ExecuteAsync()
                    .ConfigureAwait(false);

                await UpdateUserSessionAndRegisterUserAsync(result.IdToken).ConfigureAwait(false);
                return result.IdToken != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> ResetPasswordAsync()
        {
            _logger.Info("Reset password");

            var uiParent = _identityUiParentProvider.GetUiParent();
            var accounts = await GetAccountsLocalAsync().ConfigureAwait(false);

            var user = GetUserByPolicy(accounts, _applicationSettings.AuthorityPasswordReset);

            try
            {
                var result = await _publicClientApplication.AcquireTokenInteractive(_applicationSettings.Scopes)
                    .WithAccount(user)
                    .WithAuthority(_applicationSettings.AuthorityPasswordReset)
                    .WithParentActivityOrWindow(uiParent)
                    .ExecuteAsync()
                    .ConfigureAwait(false);

                await UpdateUserSessionAndRegisterUserAsync(result.IdToken).ConfigureAwait(false);
                return result.IdToken != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task SignOutAsync()
        {
            _logger.Info("Sign out");

            _userId = Guid.Empty;

            await RemoveLocalAccountsAsync().ConfigureAwait(false);
            await _cleanUpService.CleanUp().ConfigureAwait(false);
            CrossSecureStorage.Current.DeleteKey(AccessTokenKey);
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
            catch (Exception e)
            {
                _logger.Exception(e, "Failed to get userIdentifier");
            }

            return null;
        }

        private async Task UpdateUserSessionAndRegisterUserAsync(string accessToken)
        {
            if (accessToken == null)
                return;

            var accessTokenObject = new AccessToken(accessToken);
            await UpdateUserSession(accessTokenObject).ConfigureAwait(false);

            var registerUserModel = new RegisterUserModel
            {
                Id = Guid.Parse(accessTokenObject.ObjectId),
                Email = accessTokenObject.Email,
                GivenName = accessTokenObject.GivenName,
                FamilyName = accessTokenObject.FamilyName
            };

            var httpRequestResult = await _registrationUserWebService.RegisterUserAsync(registerUserModel, accessToken).ConfigureAwait(false);
            if (httpRequestResult.State != HttpRequestState.Success)
                throw new UserRegistrationFailedException();

            SetAccessToken(httpRequestResult.Payload.Token);

            await _userSubscriptionRepository.AddAsync(httpRequestResult.Payload.UserSubscription).ConfigureAwait(false);
        }

        private async Task UpdateUserSession(AccessToken accessToken)
        {
            _logger.Debug($"Update user session for '{accessToken.GivenName} {accessToken.FamilyName}' with id '{accessToken.ObjectId}'.");

            var userSession = await _userSessionRepository.GetUserSessionAsync().ConfigureAwait(false);
            userSession.ObjectId = Guid.Parse(accessToken.ObjectId);
            userSession.Email = accessToken.Email;
            userSession.GivenName = accessToken.GivenName;
            userSession.FamilyName = accessToken.FamilyName;

            ValidateUserSession(userSession);

            await _userSessionRepository.UpdateUserSessionAsync(userSession).ConfigureAwait(false);
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
