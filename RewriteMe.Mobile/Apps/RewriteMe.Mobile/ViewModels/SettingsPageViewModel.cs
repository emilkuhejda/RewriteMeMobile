using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Commands;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Interfaces.Configuration;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Localization;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.Navigation;
using RewriteMe.Mobile.Navigation.Parameters;
using RewriteMe.Mobile.Utils;
using RewriteMe.Resources.Localization;
using Xamarin.Forms;

namespace RewriteMe.Mobile.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        private readonly IUserSubscriptionService _userSubscriptionService;
        private readonly ILanguageService _languageService;
        private readonly IEmailService _emailService;
        private readonly IApplicationSettings _applicationSettings;
        private readonly IApplicationVersionProvider _applicationVersionProvider;

        private LanguageInfo _selectedLanguage;
        private string _userName;
        private string _remainingTime;
        private string _applicationVersion;

        public SettingsPageViewModel(
            IUserSubscriptionService userSubscriptionService,
            ILanguageService languageService,
            IEmailService emailService,
            IApplicationSettings applicationSettings,
            IApplicationVersionProvider applicationVersionProvider,
            IUserSessionService userSessionService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(userSessionService, dialogService, navigationService, loggerFactory)
        {
            _userSubscriptionService = userSubscriptionService;
            _languageService = languageService;
            _emailService = emailService;
            _applicationSettings = applicationSettings;
            _applicationVersionProvider = applicationVersionProvider;

            CanGoBack = true;
            HasBottomNavigation = true;

            DeveloperMode = new DeveloperMode();
            DeveloperMode.UnlockedEvent += OnUnlockedEvent;

            NavigateToLanguageCommand = new AsyncCommand(ExecuteNavigateToLanguageCommandAsync);
            NavigateToUserSettingsCommand = new AsyncCommand(ExecuteNavigateToUserSettingsCommandAsync);
            NavigateToUserSubscriptions = new AsyncCommand(ExecuteNavigateToUserSubscriptionsCommandAsync);
            NavigateToEmailCommand = new DelegateCommand(ExecuteNavigateToEmailCommand);
            NavigateToDeveloperPageCommand = new AsyncCommand(ExecuteNavigateToDeveloperPageCommandAsync);
        }

        public LanguageInfo SelectedLanguage
        {
            get => _selectedLanguage;
            set => SetProperty(ref _selectedLanguage, value);
        }

        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        public string RemainingTime
        {
            get => _remainingTime;
            set => SetProperty(ref _remainingTime, value);
        }

        public string ApplicationVersion
        {
            get => _applicationVersion;
            set => SetProperty(ref _applicationVersion, value);
        }

        public DeveloperMode DeveloperMode { get; }

        public ICommand NavigateToLanguageCommand { get; }

        public ICommand NavigateToUserSettingsCommand { get; }

        public ICommand NavigateToUserSubscriptions { get; }

        public ICommand NavigateToEmailCommand { get; }

        public ICommand NavigateToDeveloperPageCommand { get; }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                if (navigationParameters.GetNavigationMode() == NavigationMode.New)
                {
                    ApplicationVersion = _applicationVersionProvider.GetInstalledVersionNumber();
                }
                else if (navigationParameters.GetNavigationMode() == NavigationMode.Back)
                {
                    var dropDownListViewModel = navigationParameters.GetValue<DropDownListViewModel>();
                    await HandleSelectionAsync(dropDownListViewModel).ConfigureAwait(false);
                }

                UserName = await UserSessionService.GetUserNameAsync().ConfigureAwait(false);

                var remainingTime = await _userSubscriptionService.GetRemainingTimeAsync().ConfigureAwait(false);
                var sign = remainingTime.Ticks < 0 ? "-" : string.Empty;
                RemainingTime = $"{sign}{remainingTime:hh\\:mm\\:ss}";

                SelectedLanguage = await _languageService.GetLanguageInfo().ConfigureAwait(false);
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
                    await _languageService.ChangeUserLanguageAsync(language).ConfigureAwait(false);
                    break;
                default:
                    throw new NotSupportedException(nameof(SelectedLanguage));
            }
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
            var parameters = new DropDownListNavigationParameters(languages);
            navigationParameters.Add<DropDownListNavigationParameters>(parameters);

            await NavigationService.NavigateWithoutAnimationAsync(Pages.DropDownListPage, navigationParameters).ConfigureAwait(false);
        }

        private async Task ExecuteNavigateToUserSettingsCommandAsync()
        {
            await NavigationService.NavigateWithoutAnimationAsync(Pages.UserSettings).ConfigureAwait(false);
        }

        private async Task ExecuteNavigateToUserSubscriptionsCommandAsync()
        {
            await NavigationService.NavigateWithoutAnimationAsync(Pages.UserSubscriptions).ConfigureAwait(false);
        }

        private void ExecuteNavigateToEmailCommand()
        {
            ThreadHelper.InvokeOnUiThread(CreateContactUsMailAsync);
        }

        private async Task CreateContactUsMailAsync()
        {
            if (string.IsNullOrWhiteSpace(_applicationSettings.SupportMailAddress))
                return;

            if (_emailService.CanSendEmail)
            {
                var subject = $"{Loc.Text(TranslationKeys.ApplicationTitle)}";
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss \"GMT\"zzz", CultureInfo.InvariantCulture);
                var message = new StringBuilder()
                    .AppendLine()
                    .AppendLine()
                    .AppendLine()
                    .AppendLine()
                    .AppendLine()
                    .AppendLine()
                    .AppendLine()
                    .AppendLine("_______________________________________")
                    .AppendLine($"Application version: {_applicationVersionProvider.GetInstalledVersionNumber()} ({Device.RuntimePlatform})")
                    .AppendLine($"Time stamp: {timestamp}")
                    .ToString();

                _emailService.Send(_applicationSettings.SupportMailAddress, subject, message);
            }
            else
            {
                await DialogService.AlertAsync(Loc.Text(TranslationKeys.EmailIsNotSupported)).ConfigureAwait(false);
            }
        }

        private async Task ExecuteNavigateToDeveloperPageCommandAsync()
        {
            await NavigationService.NavigateWithoutAnimationAsync(Pages.Developer).ConfigureAwait(false);
        }

        private void OnUnlockedEvent(object sender, EventArgs e)
        {
            DialogService.AlertAsync(Loc.Text(TranslationKeys.DeveloperModeIsActivated));
        }

        protected override void DisposeInternal()
        {
            DeveloperMode.UnlockedEvent -= OnUnlockedEvent;
        }
    }
}
