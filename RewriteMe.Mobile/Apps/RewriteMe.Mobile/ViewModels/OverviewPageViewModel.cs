using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.LatestVersion.Abstractions;
using Plugin.Messaging;
using Prism.Commands;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Interfaces.Configuration;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.Navigation;
using RewriteMe.Mobile.Utils;
using RewriteMe.Resources.Localization;
using Xamarin.Forms;

namespace RewriteMe.Mobile.ViewModels
{
    public class OverviewPageViewModel : ViewModelBase, IDisposable
    {
        private readonly IFileItemService _fileItemService;
        private readonly IUserSessionService _userSessionService;
        private readonly IInternalValueService _internalValueService;
        private readonly ISchedulerService _schedulerService;
        private readonly ILatestVersion _latestVersion;
        private readonly IEmailTask _emailTask;
        private readonly IApplicationSettings _applicationSettings;

        private bool _isUserRegistrationSuccess;
        private IList<FileItemViewModel> _fileItems;
        private bool _disposed;

        public OverviewPageViewModel(
            IFileItemService fileItemService,
            IUserSessionService userSessionService,
            IInternalValueService internalValueService,
            ISchedulerService schedulerService,
            ILatestVersion latestVersion,
            IEmailTask emailTask,
            IApplicationSettings applicationSettings,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(dialogService, navigationService, loggerFactory)
        {
            _fileItemService = fileItemService;
            _userSessionService = userSessionService;
            _internalValueService = internalValueService;
            _schedulerService = schedulerService;
            _latestVersion = latestVersion;
            _emailTask = emailTask;
            _applicationSettings = applicationSettings;

            _schedulerService.SynchronizationCompleted += HandleSynchronizationCompleted;

            SendEmailCommand = new DelegateCommand(ExecuteSendEmailCommand);
            NavigateToCreatePageCommand = new AsyncCommand(ExecuteNavigateToCreatePageCommandAsync);
        }

        public ICommand SendEmailCommand { get; }

        public ICommand NavigateToCreatePageCommand { get; }

        public bool IsUserRegistrationSuccess
        {
            get => _isUserRegistrationSuccess;
            set => SetProperty(ref _isUserRegistrationSuccess, value);
        }

        public IList<FileItemViewModel> FileItems
        {
            get => _fileItems;
            set => SetProperty(ref _fileItems, value);
        }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                IsUserRegistrationSuccess = await _internalValueService.GetValueAsync(InternalValues.IsUserRegistrationSuccess).ConfigureAwait(false);

                await InitializeFileItems().ConfigureAwait(false);
            }
        }

        private async Task InitializeFileItems()
        {
            var fileItems = await _fileItemService.GetAllAsync().ConfigureAwait(false);
            FileItems = fileItems.OrderByDescending(x => x.DateUpdated).Select(x => new FileItemViewModel(x, NavigationService)).ToList();
        }

        private void ExecuteSendEmailCommand()
        {
            ThreadHelper.InvokeOnUiThread(CreateContactUsMailAsync);
        }

        private async Task CreateContactUsMailAsync()
        {
            if (string.IsNullOrWhiteSpace(_applicationSettings.SupportMailAddress))
                return;

            if (_emailTask.CanSendEmail)
            {
                var userId = await _userSessionService.GetUserIdAsync().ConfigureAwait(false);
                var subject = $"{Loc.Text(TranslationKeys.ApplicationTitle)} - {Loc.Text(TranslationKeys.RegistrationErrorTitle)}";
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
                    .AppendLine($"User identification: {userId}")
                    .AppendLine($"Application version: {_latestVersion.InstalledVersionNumber} ({Device.RuntimePlatform})")
                    .AppendLine($"Time stamp: {timestamp}")
                    .ToString();

                _emailTask.SendEmail(_applicationSettings.SupportMailAddress, subject, message);
            }
            else
            {
                await DialogService.AlertAsync(Loc.Text(TranslationKeys.EmailIsNotSupported)).ConfigureAwait(false);
            }
        }

        private async Task ExecuteNavigateToCreatePageCommandAsync()
        {
            await NavigationService.NavigateWithoutAnimationAsync(Pages.Create).ConfigureAwait(false);
        }

        private async void HandleSynchronizationCompleted(object sender, EventArgs e)
        {
            if (!FileItems.Any())
                return;

            if (IsCurrent)
            {
                var fileItems = await _fileItemService.GetAllAsync().ConfigureAwait(false);
                foreach (var fileItem in fileItems)
                {
                    var viewModel = FileItems.SingleOrDefault(x => x.FileItem.Id == fileItem.Id && x.FileItem.RecognitionState != fileItem.RecognitionState);
                    viewModel?.Update(fileItem);
                }
            }
            else
            {
                await InitializeFileItems().ConfigureAwait(false);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _schedulerService.SynchronizationCompleted -= HandleSynchronizationCompleted;
            }

            _disposed = true;
        }
    }
}
