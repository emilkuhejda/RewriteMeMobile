using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.WebApi.Models;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Resources.Localization;

namespace RewriteMe.Mobile.ViewModels
{
    public class DetailPageViewModel : ViewModelBase
    {
        private readonly ITranscribeItemService _transcribeItemService;

        private IList<TranscribeItemViewModel> _transcribeItems;
        private IEnumerable<ActionBarTileViewModel> _navigationItems;

        public DetailPageViewModel(
            ITranscribeItemService transcribeItemService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(dialogService, navigationService, loggerFactory)
        {
            _transcribeItemService = transcribeItemService;

            CanGoBack = true;
        }

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

        private ActionBarTileViewModel SendEmailTileItem { get; set; }

        private ActionBarTileViewModel SaveTileItem { get; set; }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                if (navigationParameters.GetNavigationMode() == NavigationMode.New)
                {
                    var fileItem = navigationParameters.GetValue<FileItem>();
                    Title = fileItem.Name;

                    var fileItemId = fileItem.Id ?? Guid.Empty;
                    var transcribeItems = await _transcribeItemService.GetAllAsync(fileItemId).ConfigureAwait(false);
                    TranscribeItems = transcribeItems.OrderBy(x => x.StartTime).Select(x => new TranscribeItemViewModel(x)).ToList();
                }

                NavigationItems = CreateNavigation();
            }
        }

        private IEnumerable<ActionBarTileViewModel> CreateNavigation()
        {
            SendEmailTileItem = new ActionBarTileViewModel
            {
                Text = Loc.Text(TranslationKeys.SendEmail),
                IsEnabled = CanExecuteSendEmailCommand(),
                IconKeyEnabled = "resource://RewriteMe.Mobile.Resources.Images.Send-Enabled.svg",
                IconKeyDisabled = "resource://RewriteMe.Mobile.Resources.Images.Send-Disabled.svg",
                SelectedCommand = new AsyncCommand(ExecuteSendEmailCommandAsync, CanExecuteSendEmailCommand)
            };

            SaveTileItem = new ActionBarTileViewModel
            {
                Text = Loc.Text(TranslationKeys.Save),
                IsEnabled = CanExecuteSaveCommand(),
                IconKeyEnabled = "resource://RewriteMe.Mobile.Resources.Images.Save-Enabled.svg",
                IconKeyDisabled = "resource://RewriteMe.Mobile.Resources.Images.Save-Disabled.svg",
                SelectedCommand = new AsyncCommand(ExecuteSaveCommandAsync, CanExecuteSaveCommand)
            };

            return new[] { SendEmailTileItem, SaveTileItem };
        }

        private bool CanExecuteSendEmailCommand()
        {
            return false;
        }

        private async Task ExecuteSendEmailCommandAsync()
        {
            await Task.CompletedTask.ConfigureAwait(false);
        }

        private bool CanExecuteSaveCommand()
        {
            return false;
        }

        private async Task ExecuteSaveCommandAsync()
        {
            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}
