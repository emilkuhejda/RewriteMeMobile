using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.LatestVersion.Abstractions;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.Navigation;
using RewriteMe.Mobile.Navigation.Parameters;
using RewriteMe.Resources.Localization;

namespace RewriteMe.Mobile.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        private readonly IInternalValueService _internalValueService;
        private readonly ILatestVersion _latestVersion;
        private readonly ILocalizer _localizer;

        private LanguageInfo _selectedLanguage;
        private string _applicationVersion;

        public SettingsPageViewModel(
            IInternalValueService internalValueService,
            ILatestVersion latestVersion,
            ILocalizer localizer,
            INavigationService navigationService)
            : base(navigationService)
        {
            _internalValueService = internalValueService;
            _latestVersion = latestVersion;
            _localizer = localizer;

            CanGoBack = true;

            NavigateToLanguageCommand = new AsyncCommand(ExecuteNavigateToLanguageCommandAsync);
        }

        public LanguageInfo SelectedLanguage
        {
            get => _selectedLanguage;
            set => SetProperty(ref _selectedLanguage, value);
        }

        public string ApplicationVersion
        {
            get => _applicationVersion;
            set => SetProperty(ref _applicationVersion, value);
        }

        public ICommand NavigateToLanguageCommand { get; }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                if (navigationParameters.GetNavigationMode() == NavigationMode.New)
                {
                    ApplicationVersion = _latestVersion.InstalledVersionNumber;
                }
                else if (navigationParameters.GetNavigationMode() == NavigationMode.Back)
                {
                    var dropDownListViewModel = navigationParameters.GetValue<DropDownListViewModel>();
                    await HandleSelectionAsync(dropDownListViewModel).ConfigureAwait(false);
                }

                await InitializeLanguageSettingAsync().ConfigureAwait(false);
            }
        }

        private async Task HandleSelectionAsync(DropDownListViewModel dropDownListViewModel)
        {
            if (dropDownListViewModel == null)
                return;

            switch (dropDownListViewModel.Type)
            {
                case nameof(SelectedLanguage):
                    var language = (CultureInfo)dropDownListViewModel.Value;
                    await ChangeUserLanguageAsync(language).ConfigureAwait(false);
                    break;
                default:
                    throw new NotSupportedException(nameof(SelectedLanguage));
            }
        }

        private async Task ChangeUserLanguageAsync(CultureInfo language)
        {
            await _internalValueService.UpdateValue(InternalValues.LanguageSetting, language.TwoLetterISOLanguageName).ConfigureAwait(false);
            _localizer.SetCultureInfo(language);
        }

        private async Task InitializeLanguageSettingAsync()
        {
            var languageName = await _internalValueService.GetValue(InternalValues.LanguageSetting).ConfigureAwait(false);
            if (languageName == null)
            {
                var currentCulture = _localizer.GetCurrentCulture();
                languageName = currentCulture.TwoLetterISOLanguageName;
            }

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
