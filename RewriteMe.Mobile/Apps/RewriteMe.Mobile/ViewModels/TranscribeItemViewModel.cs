using System;
using System.Linq;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.Mobile.ViewModels
{
    public class TranscribeItemViewModel : BindableBase
    {
        private bool _isReloadCommandVisible;
        private string _userTranscript;
        private bool _isDirty;

        public event EventHandler IsDirtyChanged;

        public TranscribeItemViewModel(TranscribeItem transcribeItem)
        {
            TranscribeItem = transcribeItem;

            if (!string.IsNullOrWhiteSpace(transcribeItem.UserTranscript))
            {
                _userTranscript = transcribeItem.UserTranscript;

                IsReloadCommandVisible = CanExecuteReloadCommand();
            }
            else if (transcribeItem.Alternatives != null && transcribeItem.Alternatives.Any())
            {
                var alternative = transcribeItem.Alternatives.First();
                _userTranscript = alternative.Transcript;
            }

            Time = $"{transcribeItem.StartTime:mm\\:ss} - {transcribeItem.EndTime:mm\\:ss}";

            ReloadCommand = new DelegateCommand(ExecuteReloadCommand, CanExecuteReloadCommand);
        }

        public ICommand ReloadCommand { get; }

        public bool IsReloadCommandVisible
        {
            get => _isReloadCommandVisible;
            set => SetProperty(ref _isReloadCommandVisible, value);
        }

        public TranscribeItem TranscribeItem { get; }

        public string Time { get; }

        public string UserTranscript
        {
            get => _userTranscript;
            set
            {
                if (SetProperty(ref _userTranscript, value))
                {
                    TranscribeItem.UserTranscript = _userTranscript;
                    IsReloadCommandVisible = CanExecuteReloadCommand();
                    IsDirty = true;
                }
            }
        }

        public bool IsDirty
        {
            get => _isDirty;
            private set
            {
                if (SetProperty(ref _isDirty, value))
                {
                    OnIsDirtyChanged();
                }
            }
        }

        private bool CanExecuteReloadCommand()
        {
            if (TranscribeItem.Alternatives == null || !TranscribeItem.Alternatives.Any())
                return false;

            var alternative = TranscribeItem.Alternatives.First();
            return !alternative.Transcript.Equals(TranscribeItem.UserTranscript);
        }

        private void ExecuteReloadCommand()
        {
            if (TranscribeItem == null || !TranscribeItem.Alternatives.Any())
                return;

            var alternative = TranscribeItem.Alternatives.First();
            UserTranscript = alternative.Transcript;
        }

        private void OnIsDirtyChanged()
        {
            IsDirtyChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
