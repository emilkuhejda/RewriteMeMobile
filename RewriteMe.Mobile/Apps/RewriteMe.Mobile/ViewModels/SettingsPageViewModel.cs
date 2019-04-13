using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.Navigation;
using RewriteMe.Mobile.Navigation.Parameters;
using RewriteMe.Resources.Localization;

namespace RewriteMe.Mobile.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        private readonly ILocalizer _localizer;

        private LanguageInfo _selectedLanguage;

        public SettingsPageViewModel(
            ILocalizer localizer,
            INavigationService navigationService)
            : base(navigationService)
        {
            _localizer = localizer;

            CanGoBack = true;

            NavigateToLanguageCommand = new AsyncCommand(ExecuteNavigateToLanguageCommandAsync);
        }

        public LanguageInfo SelectedLanguage
        {
            get => _selectedLanguage;
            set => SetProperty(ref _selectedLanguage, value);
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

                InitializeLanguageSettings();

                await Task.CompletedTask.ConfigureAwait(false);
            }
        }

        private void HandleSelection(DropDownListViewModel dropDownListViewModel)
        {
            if (dropDownListViewModel == null)
                return;

            switch (dropDownListViewModel.Type)
            {
                case nameof(SelectedLanguage):
                    _localizer.SetCultureInfo((CultureInfo)dropDownListViewModel.Value);
                    break;
                default:
                    throw new NotSupportedException(nameof(SelectedLanguage));
            }
        }

        private void InitializeLanguageSettings()
        {
            var currentCulture = _localizer.GetCurrentCulture();
            var languageName = currentCulture.TwoLetterISOLanguageName;
            var language = Languages.All.FirstOrDefault(x => x.Culture == languageName) ?? Languages.English;

            SelectedLanguage = language;
        }

        private async Task ExecuteNavigateToLanguageCommandAsync()
        {
            var languages = Languages.All.Select(x => new DropDownListViewModel
            {
                Text = x.Title,
                Value = x.GetCultureInfo(),
                Type = nameof(SelectedLanguage),
                IsSelected = x.Culture == SelectedLanguage.Culture
            });

            var navigationParameters = new NavigationParameters();
            var parameters = new DropDownListNavigationParameters(Loc.Text(TranslationKeys.Languages), languages);
            navigationParameters.Add<DropDownListNavigationParameters>(parameters);

            await NavigationService.NavigateWithoutAnimationAsync(Pages.DropDownListPage, navigationParameters).ConfigureAwait(false);
        }
    }
}
