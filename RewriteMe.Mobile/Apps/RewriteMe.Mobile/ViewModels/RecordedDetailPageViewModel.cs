using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.Messaging;
using Prism.Commands;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Domain.Transcription;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.Utils;
using RewriteMe.Resources.Localization;
using RewriteMe.Resources.Utils;

namespace RewriteMe.Mobile.ViewModels
{
    public class RecordedDetailPageViewModel : ViewModelBase
    {
        private readonly IEmailTask _emailTask;

        private IEnumerable<ActionBarTileViewModel> _navigationItems;
        private string _text;
        private bool _isDirty;

        public RecordedDetailPageViewModel(
            IEmailTask emailTask,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(dialogService, navigationService, loggerFactory)
        {
            _emailTask = emailTask;

            CanGoBack = true;

            PropertyChanged += HandlePropertyChanged;

            DeleteCommand = new AsyncCommand(ExecuteDeleteCommandAsync);
        }

        private RecordedItem RecordedItem { get; set; }

        public IEnumerable<ActionBarTileViewModel> NavigationItems
        {
            get => _navigationItems;
            set => SetProperty(ref _navigationItems, value);
        }

        public string Text
        {
            get => _text;
            set
            {
                if (SetProperty(ref _text, value))
                {
                    if (!OperationScope.IsBusy)
                    {
                        IsDirty = true;
                    }
                }
            }
        }

        public bool IsDirty
        {
            get => _isDirty;
            set => SetProperty(ref _isDirty, value);
        }

        private ActionBarTileViewModel SendTileItem { get; set; }

        private ActionBarTileViewModel SaveTileItem { get; set; }

        public ICommand DeleteCommand { get; }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                RecordedItem = navigationParameters.GetValue<RecordedItem>();
                var transcriptions = RecordedItem.AudioFiles
                    .OrderBy(x => x.DateCreated)
                    .Where(x => !string.IsNullOrWhiteSpace(x.Transcript))
                    .Select(x => x.Transcript);
                Text = string.Join(" ", transcriptions);

                InitializeNavigation();

                await Task.CompletedTask.ConfigureAwait(false);
            }
        }

        private void InitializeNavigation()
        {
            SendTileItem = new ActionBarTileViewModel
            {
                Text = Loc.Text(TranslationKeys.Send),
                IsEnabled = CanExecuteSendCommand(),
                IconKeyEnabled = "resource://RewriteMe.Mobile.Resources.Images.Send-Enabled.svg",
                IconKeyDisabled = "resource://RewriteMe.Mobile.Resources.Images.Send-Disabled.svg",
                SelectedCommand = new DelegateCommand(ExecuteSendCommand, CanExecuteSendCommand)
            };

            SaveTileItem = new ActionBarTileViewModel
            {
                Text = Loc.Text(TranslationKeys.Save),
                IsEnabled = CanExecuteSaveCommand(),
                IconKeyEnabled = "resource://RewriteMe.Mobile.Resources.Images.Save-Enabled.svg",
                IconKeyDisabled = "resource://RewriteMe.Mobile.Resources.Images.Save-Disabled.svg",
                SelectedCommand = new AsyncCommand(ExecuteSaveCommandAsync, CanExecuteSaveCommand)
            };

            NavigationItems = new[] { SendTileItem, SaveTileItem };
        }

        private bool CanExecuteSendCommand()
        {
            return _emailTask.CanSendEmail && !string.IsNullOrWhiteSpace(Text);
        }

        private void ExecuteSendCommand()
        {
            ThreadHelper.InvokeOnUiThread(SendEmailInternal);
        }

        private void SendEmailInternal()
        {
            _emailTask.SendEmail(
                subject: RecordedItem.DateCreated.ToLocalTime().ToString(Constants.TimeFormat),
                message: Text);
        }

        private bool CanExecuteSaveCommand()
        {
            return IsDirty;
        }

        private async Task ExecuteSaveCommandAsync()
        {
            await Task.CompletedTask.ConfigureAwait(false);
        }

        private async Task ExecuteDeleteCommandAsync()
        {
            var title = RecordedItem.DateCreated.ToLocalTime().ToString(Constants.TimeFormat);
            var result = await DialogService.ConfirmAsync(
                Loc.Text(TranslationKeys.PromptDeleteFileItemMessage, title),
                okText: Loc.Text(TranslationKeys.Ok),
                cancelText: Loc.Text(TranslationKeys.Cancel)).ConfigureAwait(false);

            if (result)
            {
                using (new OperationMonitor(OperationScope))
                {
                }
            }
        }

        private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsDirty))
            {
                SaveTileItem.IsEnabled = CanExecuteSaveCommand();
            }
        }
    }
}
