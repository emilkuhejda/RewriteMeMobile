using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Messaging;
using Prism.Commands;
using Prism.Navigation;
using RewriteMe.Business.Extensions;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.WebApi.Models;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.Utils;
using RewriteMe.Resources.Localization;

namespace RewriteMe.Mobile.ViewModels
{
    public class DetailPageViewModel : ViewModelBase, IDisposable
    {
        private readonly ITranscribeItemService _transcribeItemService;
        private readonly IEmailTask _emailTask;

        private IList<TranscribeItemViewModel> _transcribeItems;
        private IEnumerable<ActionBarTileViewModel> _navigationItems;
        private bool _notAvailableData;

        private bool _disposed;

        public DetailPageViewModel(
            ITranscribeItemService transcribeItemService,
            IEmailTask emailTask,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(dialogService, navigationService, loggerFactory)
        {
            _transcribeItemService = transcribeItemService;
            _emailTask = emailTask;

            CanGoBack = true;
        }

        private FileItem FileItem { get; set; }

        public IList<TranscribeItemViewModel> TranscribeItems
        {
            get => _transcribeItems;
            set => SetProperty(ref _transcribeItems, value);
        }

        public IEnumerable<ActionBarTileViewModel> NavigationItems
        {
            get => _navigationItems;
            set => SetProperty(ref _navigationItems, value);
        }

        public bool NotAvailableData
        {
            get => _notAvailableData;
            set => SetProperty(ref _notAvailableData, value);
        }

        private ActionBarTileViewModel SendTileItem { get; set; }

        private ActionBarTileViewModel SaveTileItem { get; set; }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                if (navigationParameters.GetNavigationMode() == NavigationMode.New)
                {
                    FileItem = navigationParameters.GetValue<FileItem>();
                    Title = FileItem.Name;

                    var fileItemId = FileItem.Id ?? Guid.Empty;
                    var transcribeItems = await _transcribeItemService.GetAllAsync(fileItemId).ConfigureAwait(false);

                    TranscribeItems?.ForEach(x => x.IsDirtyChanged -= HandleIsDirtyChanged);
                    TranscribeItems = transcribeItems.OrderBy(x => x.StartTime).Select(CreateTranscribeItemViewModel).ToList();

                    NotAvailableData = !TranscribeItems.Any();
                }

                NavigationItems = CreateNavigation();
            }
        }

        private TranscribeItemViewModel CreateTranscribeItemViewModel(TranscribeItem transcribeItem)
        {
            var viewModel = new TranscribeItemViewModel(transcribeItem);
            viewModel.IsDirtyChanged += HandleIsDirtyChanged;

            return viewModel;
        }

        private void HandleIsDirtyChanged(object sender, EventArgs e)
        {
            SaveTileItem.IsEnabled = CanExecuteSaveCommand();
        }

        private IEnumerable<ActionBarTileViewModel> CreateNavigation()
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

            return new[] { SendTileItem, SaveTileItem };
        }

        private bool CanExecuteSendCommand()
        {
            return _emailTask.CanSendEmail;
        }

        private void ExecuteSendCommand()
        {
            ThreadHelper.InvokeOnUiThread(SendEmailInternal);
        }

        private void SendEmailInternal()
        {
            var message = new StringBuilder();
            foreach (var transcribeItem in TranscribeItems)
            {
                message.AppendLine(transcribeItem.UserTranscript);
                message.AppendLine(transcribeItem.Time);
                message.AppendLine().AppendLine();
            }

            _emailTask.SendEmail(
                subject: FileItem.Name,
                message: message.ToString());
        }

        private bool CanExecuteSaveCommand()
        {
            return TranscribeItems.Any(x => x.IsDirty);
        }

        private async Task ExecuteSaveCommandAsync()
        {
            var transcribeItemsToSave = TranscribeItems.Where(x => x.IsDirty).Select(x => x.TranscribeItem);

            await _transcribeItemService.SaveAndSendAsync(transcribeItemsToSave).ConfigureAwait(false);
            await NavigationService.GoBackWithoutAnimationAsync().ConfigureAwait(false);
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
                TranscribeItems?.ForEach(x => x.IsDirtyChanged -= HandleIsDirtyChanged);
            }

            _disposed = true;
        }
    }
}
