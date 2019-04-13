using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.Navigation;
using RewriteMe.Mobile.Navigation.Parameters;

namespace RewriteMe.Mobile.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        public SettingsPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            CanGoBack = true;

            NavigateToLanguageCommand = new AsyncCommand(ExecuteNavigateToLanguageCommandAsync);
        }

        public ICommand NavigateToLanguageCommand { get; }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                if (navigationParameters.GetNavigationMode() == NavigationMode.Back)
                {
                    var dropDownListViewModel = navigationParameters.GetValue<DropDownListViewModel>();
                    HandleSelection(dropDownListViewModel);
                }

                await Task.CompletedTask.ConfigureAwait(false);
            }
        }

        private void HandleSelection(DropDownListViewModel dropDownListViewModel)
        {
            if (dropDownListViewModel == null)
                return;
        }

        private async Task ExecuteNavigateToLanguageCommandAsync()
        {
            var list = new List<DropDownListViewModel>
            {
                new DropDownListViewModel
                {
                    Text = "English",
                    Value = new CultureInfo("en")
                },
                new DropDownListViewModel
                {
                    Text = "Slovak",
                    Value = new CultureInfo("sk"),
                    IsSelected = true
                }
            };

            var navigationParameters = new NavigationParameters();
            var parameters = new DropDownListNavigationParameters("Languages", list);
            navigationParameters.Add<DropDownListNavigationParameters>(parameters);

            await NavigationService.NavigateWithoutAnimationAsync(Pages.DropDownListPage, navigationParameters).ConfigureAwait(false);
        }
    }
}
