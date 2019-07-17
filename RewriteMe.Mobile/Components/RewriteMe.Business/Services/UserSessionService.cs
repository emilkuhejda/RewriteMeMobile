using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using RewriteMe.Business.Configuration;
using RewriteMe.Business.Wrappers;
using RewriteMe.Domain.Configuration;
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
        private readonly IRegistrationUserWebService _registrationUserWebService;
        private readonly IInternalValueService _internalValueService;
        private readonly ICleanUpService _cleanUpService;
        private readonly IPublicClientApplication _publicClientApplication;
        private readonly IIdentityUiParentProvider _identityUiParentProvider;
        private readonly IApplicationSettings _applicationSettings;
        private readonly IUserSessionRepository _userSessionRepository;
        private readonly IUserSubscriptionRepository _userSubscriptionRepository;
        private readonly ILogger _logger;

        private Guid _userId = Guid.Empty;
        private string _accessToken;

        public UserSessionService(
            IRegistrationUserWebService registrationUserWebService,
            IInternalValueService internalValueService,
            ICleanUpService cleanUpService,
            IPublicClientApplicationFactory publicClientApplicationFactory,
            IIdentityUiParentProvider identityUiParentProvider,
            IApplicationSettings applicationSettings,
            IUserSessionRepository userSessionRepository,
            IUserSubscriptionRepository userSubscriptionRepository,
            ILoggerFactory loggerFactory)
        {
            _registrationUserWebService = registrationUserWebService;
            _internalValueService = internalValueService;
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

        public async Task<string> GetAccessTokenSilentAsync()
        {
            if (!string.IsNullOrEmpty(_accessToken))
                return _accessToken;

            var accessToken = await GetAccessTokenSilentAsync(_applicationSettings.PolicySignUpSignIn, _applicationSettings.AuthoritySignUpSignIn).ConfigureAwait(false);
            if (accessToken == null)
            {
                accessToken = await GetAccessTokenSilentAsync(_applicationSettings.PolicySignIn, _applicationSettings.AuthoritySignIn).ConfigureAwait(false);
            }

            await UpdateUserSessionAndRegisterUserAsync(accessToken).ConfigureAwait(false);
            return accessToken;
        }

        private async Task<string> GetAccessTokenSilentAsync(string policy, string authority)
        {
            var accounts = await GetAccountsLocalAsync().ConfigureAwait(false);

            var user = GetUserByPolicy(accounts, policy);
            if (user != null)
            {
                try
                {
                    var result = await _publicClientApplication.AcquireTokenSilent(_applicationSettings.Scopes, user)
                        .WithAuthority(authority)
                        .ExecuteAsync()
                        .ConfigureAwait(false);
                    return result.IdToken;
                }
                catch (Exception e)
                {
                    // Ignore exception
                    _logger.Exception(e, "Could not retrieve access token silent.");
                }
            }

            return null;
        }

        public async Task<bool> IsSignedInAsync()
        {
            IEnumerable<IAccount> account;

            var offline = false;
            try
            {
                account = await _publicClientApplication.GetAccountsAsync().ConfigureAwait(false);
            }
            catch (HttpRequestException)
            {
                // MSAL 2.x throws HttpRequestException when not connected to the internet, this is a workaround!
                offline = true;
                account = new List<IAccount>();
            }

            var userExists = account.Any();
            var userSessionExists = await _userSessionRepository.UserSessionExistsAsync().ConfigureAwait(false);
            var isSignedIn = (userExists || offline) && userSessionExists;
            return isSignedIn;
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
            catch (MsalServiceException e)
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
            catch (MsalException e)
            {
                if ("authentication_canceled".Equals(e.ErrorCode, StringComparison.Ordinal))
                {
                    // User has canceled the sign in or registering
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
            _accessToken = null;

            await RemoveLocalAccountsAsync().ConfigureAwait(false);
            await _cleanUpService.CleanUp().ConfigureAwait(false);
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

            var isUserRegistered = await _internalValueService.GetValueAsync(InternalValues.IsUserRegistrationSuccess).ConfigureAwait(false);
            if (accessTokenObject.NewUser && !isUserRegistered)
            {
                var registerUserModel = new RegisterUserModel
                {
                    Id = Guid.Parse(accessTokenObject.ObjectId),
                    Email = accessTokenObject.Email,
                    GivenName = accessTokenObject.GivenName,
                    FamilyName = accessTokenObject.FamilyName
                };

                var httpRequestResult = await _registrationUserWebService.RegisterUserAsync(registerUserModel).ConfigureAwait(false);
                if (httpRequestResult.State == HttpRequestState.Success)
                {
                    await _userSubscriptionRepository.AddAsync(httpRequestResult.Payload).ConfigureAwait(false);
                    await _internalValueService.UpdateValueAsync(InternalValues.IsUserRegistrationSuccess, true).ConfigureAwait(false);
                }
            }
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
