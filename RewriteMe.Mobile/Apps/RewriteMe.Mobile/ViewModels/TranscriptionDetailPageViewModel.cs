using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Commands;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Interfaces.Managers;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.WebApi;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Controls;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Resources.Localization;
using Xamarin.Forms;

namespace RewriteMe.Mobile.ViewModels
{
    public class TranscriptionDetailPageViewModel : ViewModelBase
    {
        private readonly ITranscribeItemManager _transcribeItemManager;
        private readonly ITranscriptAudioSourceService _transcriptAudioSourceService;

        private IEnumerable<LabelComponent> _items;
        private string _text;
        private bool _isMultiLabelVisible;
        private Color _textColor;
        private FormattedString _formattedText;

        public TranscriptionDetailPageViewModel(
            ITranscribeItemManager transcribeItemManager,
            ITranscriptAudioSourceService transcriptAudioSourceService,
            IUserSessionService userSessionService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(userSessionService, dialogService, navigationService, loggerFactory)
        {
            _transcribeItemManager = transcribeItemManager;
            _transcriptAudioSourceService = transcriptAudioSourceService;

            CanGoBack = true;

            PlayerViewModel = new PlayerViewModel();
            PlayerViewModel.Tick += OnTick;

            TapCommand = new DelegateCommand(ExecuteTapCommand);
        }

        public ICommand TapCommand { get; set; }

        public PlayerViewModel PlayerViewModel { get; }

        private LabelComponent CurrentComponent { get; set; }

        public IEnumerable<LabelComponent> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }

        public bool IsMultiLabelVisible
        {
            get => _isMultiLabelVisible;
            set => SetProperty(ref _isMultiLabelVisible, value);
        }

        public Color TextColor
        {
            get => _textColor;
            set => SetProperty(ref _textColor, value);
        }

        public FormattedString FormattedText
        {
            get => _formattedText;
            set => SetProperty(ref _formattedText, value);
        }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                var transcribeItem = navigationParameters.GetValue<TranscribeItem>();
                var transcriptAudioSource = await _transcriptAudioSourceService.GetAsync(transcribeItem.Id).ConfigureAwait(false);
                if (transcriptAudioSource == null)
                {
                    var errorMessage = _transcribeItemManager.IsRunning
                        ? Loc.Text(TranslationKeys.SynchronizationInProgressErrorMessage)
                        : Loc.Text(TranslationKeys.TranscribeAudioSourceNotFoundErrorMessage);

                    await DialogService.AlertAsync(errorMessage).ConfigureAwait(false);
                    await NavigationService.GoBackWithoutAnimationAsync().ConfigureAwait(false);
                    return;
                }

                if (transcriptAudioSource.Source == null || !transcriptAudioSource.Source.Any())
                {
                    await DialogService.AlertAsync(Loc.Text(TranslationKeys.TranscribeAudioSourceNotFoundErrorMessage)).ConfigureAwait(false);
                    await NavigationService.GoBackWithoutAnimationAsync().ConfigureAwait(false);
                    return;
                }

                PlayerViewModel.Load(transcriptAudioSource.Source);

                IEnumerable<RecognitionWordInfo> words = transcribeItem.Alternatives.SelectMany(x => x.Words).ToList();
                Items = words.OrderBy(x => x.StartTimeTicks).Select(x => new LabelComponent
                {
                    Text = x.Word,
                    StartTime = x.StartTime
                }).ToList();

                Text = string.Join(string.Empty, transcribeItem.Alternatives.Select(x => x.Transcript));
                IsMultiLabelVisible = true;
                TextColor = Color.Transparent;
            }
        }

        private void OnTick(object sender, EventArgs e)
        {
            var position = TimeSpan.FromSeconds(PlayerViewModel.CurrentPosition);
            var item = Items.LastOrDefault(x => position >= x.StartTime);

            if (item == null)
                return;

            if (CurrentComponent != null)
            {
                CurrentComponent.IsHighlighted = false;
            }

            item.IsHighlighted = true;
            CurrentComponent = item;
        }

        private void ExecuteTapCommand()
        {
        }
    }
}
