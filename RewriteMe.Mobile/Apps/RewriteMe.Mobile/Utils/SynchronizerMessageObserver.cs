using RewriteMe.Domain.Events;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Interfaces.Utils;
using RewriteMe.Resources.Localization;

namespace RewriteMe.Mobile.Utils
{
    public class SynchronizerMessageObserver : ISynchronizerMessageObserver
    {
        private readonly ISynchronizerService _synchronizerService;
        private readonly IDialogService _dialogService;
        private bool _isStarted;

        public SynchronizerMessageObserver(
            ISynchronizerService synchronizerService,
            IDialogService dialogService)
        {
            _synchronizerService = synchronizerService;
            _dialogService = dialogService;
        }

        public void Start()
        {
            if (_isStarted)
                return;

            _synchronizerService.RecognitionErrorOccurred += HandleRecognitionErrorOccurred;

            _isStarted = true;
        }

        private async void HandleRecognitionErrorOccurred(object sender, RecognitionErrorOccurredEventArgs e)
        {
            await _dialogService.AlertAsync(Loc.Text(TranslationKeys.TranscriptionErrorMessage, e.FileName)).ConfigureAwait(false);
        }
    }
}
