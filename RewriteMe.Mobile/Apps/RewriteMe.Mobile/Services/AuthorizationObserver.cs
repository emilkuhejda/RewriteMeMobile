using System;
using System.Threading.Tasks;
using Prism.Navigation;
using RewriteMe.Domain.Interfaces.Managers;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Interfaces.Utils;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.Navigation;

namespace RewriteMe.Mobile.Services
{
    public class AuthorizationObserver : IAuthorizationObserver
    {
        private readonly ISynchronizerService _synchronizerService;
        private readonly ITranscribeItemManager _transcribeItemManager;
        private readonly IUserSessionService _userSessionService;

        private INavigationService _navigationService;

        public AuthorizationObserver(
            ISynchronizerService synchronizerService,
            ITranscribeItemManager transcribeItemManager,
            IUserSessionService userSessionService)
        {
            _synchronizerService = synchronizerService;
            _transcribeItemManager = transcribeItemManager;
            _userSessionService = userSessionService;
        }

        public void Initialize(INavigationService navigationService)
        {
            _navigationService = navigationService;

            _synchronizerService.UnauthorizedCallOccurred -= HandleUnauthorizedCallOccurred;
            _transcribeItemManager.UnauthorizedCallOccurred -= HandleUnauthorizedCallOccurred;

            _synchronizerService.UnauthorizedCallOccurred += HandleUnauthorizedCallOccurred;
            _transcribeItemManager.UnauthorizedCallOccurred += HandleUnauthorizedCallOccurred;
        }

        public async Task LogOutAsync()
        {
            if (_navigationService == null)
                throw new InvalidOperationException("Navigation service is not initialized.");

            await _userSessionService.SignOutAsync().ConfigureAwait(false);
            await _navigationService.NavigateWithoutAnimationAsync($"/{Pages.Navigation}/{Pages.Login}").ConfigureAwait(false);
        }

        private async void HandleUnauthorizedCallOccurred(object sender, EventArgs e)
        {
            await LogOutAsync().ConfigureAwait(false);
        }
    }
}
