using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Navigation;
using RewriteMe.Domain.Extensions;
using RewriteMe.Domain.Localization;
using RewriteMe.Domain.WebApi.Models;
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

            Initialize(languageInfo);

            NavigateToDetailPageCommand = new AsyncCommand(ExecuteNavigateToDetailPageCommandAsync);
        }

        public string Title { get; private set; }

        public ICommand NavigateToDetailPageCommand { get; }

        private void Initialize(LanguageInfo languageInfo)
        {
            var currentLanguage = languageInfo.ToLanguage();
            var languageVersion = _informationMessage.LanguageVersions?.FirstOrDefault(x => x.Language == currentLanguage);
            if (languageVersion != null)
            {
                Title = languageVersion.Title;
            }
        }

        private async Task ExecuteNavigateToDetailPageCommandAsync()
        {
            var navigationParameters = new NavigationParameters();
            navigationParameters.Add<InformationMessage>(_informationMessage);
            await _navigationService.NavigateWithoutAnimationAsync(Pages.DetailInformationMessage, navigationParameters).ConfigureAwait(false);
        }
    }
}
