using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Transcription;
using RewriteMe.Domain.WebApi.Models;
using RewriteMe.Resources.Localization;

namespace RewriteMe.Mobile.ViewModels
{
    public class TranscribeItemViewModel : DetailItemViewModel<TranscribeItem>
    {
        private readonly ITranscriptAudioSourceService _transcriptAudioSourceService;
        private readonly IRewriteMeWebService _rewriteMeWebService;
        private readonly CancellationToken _cancellationToken;

        public TranscribeItemViewModel(
            ITranscriptAudioSourceService transcriptAudioSourceService,
            IRewriteMeWebService rewriteMeWebService,
            IDialogService dialogService,
            PlayerViewModel playerViewModel,
            TranscribeItem transcribeItem,
            CancellationToken cancellationToken)
            : base(playerViewModel, dialogService, transcribeItem)
        {
            _transcriptAudioSourceService = transcriptAudioSourceService;
            _rewriteMeWebService = rewriteMeWebService;
            _cancellationToken = cancellationToken;

            if (!string.IsNullOrWhiteSpace(transcribeItem.UserTranscript))
            {
                SetTranscript(transcribeItem.UserTranscript);
                IsReloadCommandVisible = CanExecuteReload();
            }
            else if (transcribeItem.Alternatives != null && transcribeItem.Alternatives.Any())
            {
                var alternative = transcribeItem.Alternatives.First();
                SetTranscript(alternative.Transcript);
            }

            Time = $"{transcribeItem.StartTime:mm\\:ss} - {transcribeItem.EndTime:mm\\:ss}";
        }

        protected override void OnTranscriptChanged(string transcript)
        {
            DetailItem.UserTranscript = transcript;
        }

        protected override bool CanExecuteReloadCommand()
        {
            return CanExecuteReload();
        }

        private bool CanExecuteReload()
        {
            if (DetailItem.Alternatives == null || !DetailItem.Alternatives.Any())
                return false;

            var alternative = DetailItem.Alternatives.First();
            return !alternative.Transcript.Equals(DetailItem.UserTranscript, StringComparison.Ordinal);
        }

        protected override async Task ExecutePlayCommandAsync()
        {
            using (new OperationMonitor(OperationScope))
            {
                var transcriptAudioSource = await _transcriptAudioSourceService.GetAsync(DetailItem.Id).ConfigureAwait(false);
                var source = transcriptAudioSource?.Source;

                if (transcriptAudioSource == null)
                {
                    var httpRequestResult = await _rewriteMeWebService.GetTranscribeAudioSourceAsync(DetailItem.Id, _cancellationToken).ConfigureAwait(false);
                    if (httpRequestResult.State == HttpRequestState.Success)
                    {
                        source = httpRequestResult.Payload;

                        var audioSource = new TranscriptAudioSource
                        {
                            Id = Guid.NewGuid(),
                            TranscribeItemId = DetailItem.Id,
                            Source = source
                        };

                        await _transcriptAudioSourceService.InsertAsync(audioSource).ConfigureAwait(false);
                    }
                }

                if (source == null)
                {
                    await DialogService.AlertAsync(Loc.Text(TranslationKeys.TranscribeAudioSourceNotFoundErrorMessage)).ConfigureAwait(false);
                    return;
                }

                PlayerViewModel.Load(source);
                PlayerViewModel.Play();
            }
        }

        protected override void ExecuteReloadCommand()
        {
            if (DetailItem == null || !DetailItem.Alternatives.Any())
                return;

            var alternative = DetailItem.Alternatives.First();
            Transcript = alternative.Transcript;
        }
    }
}
