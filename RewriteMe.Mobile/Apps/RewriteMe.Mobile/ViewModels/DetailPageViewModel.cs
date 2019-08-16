using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Navigation;
using RewriteMe.Business.Extensions;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.WebApi.Models;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Resources.Localization;

namespace RewriteMe.Mobile.ViewModels
{
    public class DetailPageViewModel : DetailBaseViewModel<TranscribeItem>
    {
        private readonly ITranscribeItemService _transcribeItemService;
        private readonly ITranscriptAudioSourceService _transcriptAudioSourceService;
        private readonly IFileItemService _fileItemService;
        private readonly IRewriteMeWebService _rewriteMeWebService;

        public DetailPageViewModel(
            ITranscribeItemService transcribeItemService,
            ITranscriptAudioSourceService transcriptAudioSourceService,
            IFileItemService fileItemService,
            IRewriteMeWebService rewriteMeWebService,
            IEmailService emailService,
            IUserSessionService userSessionService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(emailService, userSessionService, dialogService, navigationService, loggerFactory)
        {
            _transcribeItemService = transcribeItemService;
            _transcriptAudioSourceService = transcriptAudioSourceService;
            _fileItemService = fileItemService;
            _rewriteMeWebService = rewriteMeWebService;
        }

        private FileItem FileItem { get; set; }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                if (navigationParameters.GetNavigationMode() == NavigationMode.New)
                {
                    FileItem = navigationParameters.GetValue<FileItem>();

                    var transcribeItems = await _transcribeItemService.GetAllAsync(FileItem.Id).ConfigureAwait(false);

                    DetailItems?.ForEach(x => x.IsDirtyChanged -= HandleIsDirtyChanged);
                    DetailItems = transcribeItems.OrderBy(x => x.StartTime).Select(CreateDetailItemViewModel).ToList();

                    NotAvailableData = !DetailItems.Any();
                }

                NavigationItems = CreateNavigation();
            }
        }

        private DetailItemViewModel<TranscribeItem> CreateDetailItemViewModel(TranscribeItem detailItem)
        {
            var viewModel = new TranscribeItemViewModel(
                _transcriptAudioSourceService,
                _rewriteMeWebService,
                DialogService,
                PlayerViewModel,
                detailItem,
                CancellationToken);
            viewModel.IsDirtyChanged += HandleIsDirtyChanged;

            return viewModel;
        }

        protected override void SendEmailInternal()
        {
            var message = new StringBuilder();
            foreach (var transcribeItem in DetailItems)
            {
                message.AppendLine(transcribeItem.Transcript);
                message.AppendLine(transcribeItem.Time);
                message.AppendLine().AppendLine();
            }

            EmailService.Send(
                subject: FileItem.Name,
                message: message.ToString());
        }

        protected override bool CanExecuteSaveCommand()
        {
            return DetailItems.Any(x => x.IsDirty);
        }

        protected override async Task ExecuteSaveCommandAsync()
        {
            var transcribeItemsToSave = DetailItems.Where(x => x.IsDirty).Select(x => x.DetailItem);

            await _transcribeItemService.SaveAndSendAsync(transcribeItemsToSave).ConfigureAwait(false);
            await NavigationService.GoBackWithoutAnimationAsync().ConfigureAwait(false);
        }

        protected override async Task ExecuteDeleteCommandAsync()
        {
            var result = await DialogService.ConfirmAsync(
                Loc.Text(TranslationKeys.PromptDeleteFileItemMessage, FileItem.Name),
                okText: Loc.Text(TranslationKeys.Ok),
                cancelText: Loc.Text(TranslationKeys.Cancel)).ConfigureAwait(false);

            if (result)
            {
                using (new OperationMonitor(OperationScope))
                {
                    await _fileItemService.DeleteAsync(FileItem).ConfigureAwait(false);
                    await NavigationService.GoBackWithoutAnimationAsync().ConfigureAwait(false);
                }
            }
        }
    }
}
