using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Navigation;
using RewriteMe.Domain.Extensions;
using RewriteMe.Domain.Localization;
using RewriteMe.Domain.WebApi;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.Navigation;

namespace RewriteMe.Mobile.ViewModels
{
    public class InformationMessageViewModel
    {
        private readonly InformationMessage _informationMessage;
        private readonly INavigationService _navigationService;

        public InformationMessageViewModel(InformationMessage informationMessage, LanguageInfo languageInfo, INavigationService navigationService)
        {
            _informationMessage = informationMessage;
            _navigationService = navigationService;

            Initialize(informationMessage, languageInfo);

            NavigateToDetailPageCommand = new AsyncCommand(ExecuteNavigateToDetailPageCommandAsync);
        }

        public Guid Id => _informationMessage.Id;

        public string Title { get; private set; }

        public bool WasOpened { get; private set; }

        public ICommand NavigateToDetailPageCommand { get; }

        private void Initialize(InformationMessage informationMessage, LanguageInfo languageInfo)
        {
            var currentLanguage = languageInfo.ToLanguage();
            var languageVersion = informationMessage.LanguageVersions?.FirstOrDefault(x => x.Language == currentLanguage);
            if (languageVersion != null)
            {
                Title = languageVersion.Title;
            }

            WasOpened = informationMessage.WasOpened;
        }

        private async Task ExecuteNavigateToDetailPageCommandAsync()
        {
            var navigationParameters = new NavigationParameters();
            navigationParameters.Add<InformationMessage>(_informationMessage);
            await _navigationService.NavigateWithoutAnimationAsync(Pages.DetailInfo, navigationParameters).ConfigureAwait(false);
        }
    }
}
