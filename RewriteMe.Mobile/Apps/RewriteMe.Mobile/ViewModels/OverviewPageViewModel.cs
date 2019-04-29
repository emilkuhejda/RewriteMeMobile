﻿using System;
using System.Globalization;
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
using RewriteMe.Mobile.Utils;
using RewriteMe.Resources.Localization;
using Xamarin.Forms;

namespace RewriteMe.Mobile.ViewModels
{
    public class OverviewPageViewModel : ViewModelBase
    {
        private readonly IUserSessionService _userSessionService;
        private readonly IInternalValueService _internalValueService;
        private readonly ILatestVersion _latestVersion;
        private readonly IEmailTask _emailTask;
        private readonly IApplicationSettings _applicationSettings;

        private bool _isUserRegistrationSuccess;

        public OverviewPageViewModel(
            IUserSessionService userSessionService,
            IInternalValueService internalValueService,
            ILatestVersion latestVersion,
            IEmailTask emailTask,
            IApplicationSettings applicationSettings,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(dialogService, navigationService, loggerFactory)
        {
            _userSessionService = userSessionService;
            _internalValueService = internalValueService;
            _latestVersion = latestVersion;
            _emailTask = emailTask;
            _applicationSettings = applicationSettings;

            SendEmailCommand = new DelegateCommand(ExecuteSendEmailCommand);
        }

        public ICommand SendEmailCommand { get; }

        public bool IsUserRegistrationSuccess
        {
            get => _isUserRegistrationSuccess;
            set => SetProperty(ref _isUserRegistrationSuccess, value);
        }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                IsUserRegistrationSuccess = await _internalValueService.GetValueAsync(InternalValues.IsUserRegistrationSuccess).ConfigureAwait(false);
            }
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
                    .AppendLine($"{Loc.Text(TranslationKeys.UserIdentificationLabel)} {userId}")
                    .AppendLine($"{Loc.Text(TranslationKeys.ApplicationVersion)} {_latestVersion.InstalledVersionNumber} ({Device.RuntimePlatform})")
                    .AppendLine($"{Loc.Text(TranslationKeys.TimeStamp)} {timestamp}")
                    .ToString();

                _emailTask.SendEmail(_applicationSettings.SupportMailAddress, subject, message);
            }
            else
            {
                await DialogService.AlertAsync(Loc.Text(TranslationKeys.EmailIsNotSupported)).ConfigureAwait(false);
            }
        }
    }
}