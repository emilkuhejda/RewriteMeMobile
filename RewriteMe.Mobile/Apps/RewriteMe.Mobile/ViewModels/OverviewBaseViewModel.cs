﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.LatestVersion.Abstractions;
using Prism.Commands;
using Prism.Navigation;
using RewriteMe.Domain.Interfaces.Configuration;
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
    public abstract class OverviewBaseViewModel : ViewModelBase
    {
        private readonly ISynchronizationService _synchronizationService;
        private readonly IEmailService _emailService;
        private readonly ILatestVersion _latestVersion;
        private readonly IApplicationSettings _applicationSettings;

        private IEnumerable<ActionBarTileViewModel> _navigationItems;
        private bool _isNotUserRegistrationSuccess;
        private bool _notAvailableData;
        private bool _isRefreshing;

        protected OverviewBaseViewModel(
            ISynchronizationService synchronizationService,
            IInternalValueService internalValueService,
            IEmailService emailService,
            ILatestVersion latestVersion,
            IApplicationSettings applicationSettings,
            IUserSessionService userSessionService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(userSessionService, dialogService, navigationService, loggerFactory)
        {
            _synchronizationService = synchronizationService;
            _emailService = emailService;
            _latestVersion = latestVersion;
            _applicationSettings = applicationSettings;

            InternalValueService = internalValueService;

            SendEmailCommand = new DelegateCommand(ExecuteSendEmailCommand);
            NavigateToRecorderCommand = new AsyncCommand(ExecuteNavigateToRecorderCommandAsync);
            RefreshCommand = new AsyncCommand(ExecuteRefreshCommandAsync);
        }

        protected IInternalValueService InternalValueService { get; }

        public IEnumerable<ActionBarTileViewModel> NavigationItems
        {
            get => _navigationItems;
            set => SetProperty(ref _navigationItems, value);
        }

        public bool IsNotUserRegistrationSuccess
        {
            get => _isNotUserRegistrationSuccess;
            set => SetProperty(ref _isNotUserRegistrationSuccess, value);
        }

        public bool NotAvailableData
        {
            get => _notAvailableData;
            set => SetProperty(ref _notAvailableData, value);
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public ICommand SendEmailCommand { get; }

        public ICommand NavigateToRecorderCommand { get; }

        public ICommand RefreshCommand { get; }

        protected void InitializeNavigation(bool isOverview)
        {
            var audioFilesNavigationItem = new ActionBarTileViewModel
            {
                Text = Loc.Text(TranslationKeys.AudioFiles),
                IsEnabled = !isOverview,
                IconKeyEnabled = "resource://RewriteMe.Mobile.Resources.Images.Overview-Enabled.svg",
                IconKeyDisabled = "resource://RewriteMe.Mobile.Resources.Images.Overview-Disabled.svg",
                SelectedCommand = new AsyncCommand(ExecuteNavigateToOverviewAsync)
            };

            var recordedFilesNavigationItem = new ActionBarTileViewModel
            {
                Text = Loc.Text(TranslationKeys.RecordedFiles),
                IsEnabled = isOverview,
                IconKeyEnabled = "resource://RewriteMe.Mobile.Resources.Images.RecorderOverview-Enabled.svg",
                IconKeyDisabled = "resource://RewriteMe.Mobile.Resources.Images.RecorderOverview-Disabled.svg",
                SelectedCommand = new AsyncCommand(ExecuteNavigateToRecorderOverviewAsync)
            };

            NavigationItems = new[] { audioFilesNavigationItem, recordedFilesNavigationItem };
        }

        private void ExecuteSendEmailCommand()
        {
            ThreadHelper.InvokeOnUiThread(CreateContactUsMailAsync);
        }

        private async Task CreateContactUsMailAsync()
        {
            if (string.IsNullOrWhiteSpace(_applicationSettings.SupportMailAddress))
                return;

            if (_emailService.CanSendEmail)
            {
                var userId = await UserSessionService.GetUserIdAsync().ConfigureAwait(false);
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

                _emailService.Send(_applicationSettings.SupportMailAddress, subject, message);
            }
            else
            {
                await DialogService.AlertAsync(Loc.Text(TranslationKeys.EmailIsNotSupported)).ConfigureAwait(false);
            }
        }

        private async Task ExecuteNavigateToRecorderCommandAsync()
        {
            await NavigationService.NavigateWithoutAnimationAsync(Pages.Recorder).ConfigureAwait(false);
        }

        private async Task ExecuteRefreshCommandAsync()
        {
            IsRefreshing = true;

            await _synchronizationService.InitializeAsync().ConfigureAwait(false);

            IsRefreshing = false;
        }

        protected abstract Task ExecuteNavigateToOverviewAsync();

        protected abstract Task ExecuteNavigateToRecorderOverviewAsync();
    }
}
