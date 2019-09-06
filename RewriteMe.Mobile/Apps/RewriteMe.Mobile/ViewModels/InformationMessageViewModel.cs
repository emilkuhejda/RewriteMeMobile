using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Navigation;
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

        public InformationMessageViewModel(InformationMessage informationMessage, INavigationService navigationService)
        {
            _informationMessage = informationMessage;
            _navigationService = navigationService;

            if (informationMessage.LanguageVersions.Any())
            {
                var languageVersion = informationMessage.LanguageVersions.First();
                Title = languageVersion.Title;
            }

            NavigateToDetailPageCommand = new AsyncCommand(ExecuteNavigateToDetailPageCommandAsync);
        }

        public string Title { get; }

        public ICommand NavigateToDetailPageCommand { get; }

        private async Task ExecuteNavigateToDetailPageCommandAsync()
        {
            var navigationParameters = new NavigationParameters();
            navigationParameters.Add<InformationMessage>(_informationMessage);
            await _navigationService.NavigateWithoutAnimationAsync(Pages.DetailInformationMessage, navigationParameters).ConfigureAwait(false);
        }
    }
}
