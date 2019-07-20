using System.Linq;
using System.Threading.Tasks;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.WebApi.Models;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.Transcription;
using RewriteMe.Resources.Localization;

namespace RewriteMe.Mobile.ViewModels
{
    public class TranscribePageViewModel : TranscribeBaseViewModel
    {
        public TranscribePageViewModel(
            IFileItemService fileItemService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(fileItemService, dialogService, navigationService, loggerFactory)
        {
        }

        private FileItem FileItem { get; set; }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            InitializeNavigationItems();

            using (new OperationMonitor(OperationScope))
            {
                if (navigationParameters.GetNavigationMode() == NavigationMode.New)
                {
                    FileItem = navigationParameters.GetValue<FileItem>();
                    Name = FileItem.Name;
                    SelectedLanguage = SupportedLanguages.All.FirstOrDefault(x => x.Culture == FileItem.Language);

                    CanTranscribe = await FileItemService.CanTranscribeAsync(FileItem.TotalTime).ConfigureAwait(false);
                }
                else if (navigationParameters.GetNavigationMode() == NavigationMode.Back)
                {
                    var dropDownListViewModel = navigationParameters.GetValue<DropDownListViewModel>();
                    HandleSelectionAsync(dropDownListViewModel);
                }
            }
        }

        protected override async Task ExecuteTranscribeInternalAsync()
        {
            await FileItemService.TranscribeAsync(FileItem.Id, SelectedLanguage.Culture).ConfigureAwait(false);
            await NavigationService.GoBackWithoutAnimationAsync().ConfigureAwait(false);
        }

        protected override async Task ExecuteDeleteInternalAsync()
        {
            var result = await DialogService.ConfirmAsync(
                Loc.Text(TranslationKeys.PromptDeleteFileItemMessage, FileItem.Name),
                okText: Loc.Text(TranslationKeys.Ok),
                cancelText: Loc.Text(TranslationKeys.Cancel)).ConfigureAwait(false);

            if (result)
            {
                using (new OperationMonitor(OperationScope))
                {
                    await FileItemService.DeleteAsync(FileItem).ConfigureAwait(false);
                    await NavigationService.GoBackWithoutAnimationAsync().ConfigureAwait(false);
                }
            }
        }
    }
}
