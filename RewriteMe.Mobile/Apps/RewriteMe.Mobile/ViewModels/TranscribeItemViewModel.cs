using System;
using System.Linq;
using Prism.Mvvm;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.Mobile.ViewModels
{
    public class TranscribeItemViewModel : BindableBase
    {
        private string _userTranscript;
        private bool _isDirty;

        public event EventHandler IsDirtyChanged;

        public TranscribeItemViewModel(TranscribeItem transcribeItem)
        {
            TranscribeItem = transcribeItem;

            if (!string.IsNullOrWhiteSpace(transcribeItem.UserTranscript))
            {
                _userTranscript = transcribeItem.UserTranscript;
            }
            else if (transcribeItem.Alternatives != null && transcribeItem.Alternatives.Any())
            {
                var alternative = transcribeItem.Alternatives.First();
                _userTranscript = alternative.Transcript;
            }

            TimeSpan.TryParse(transcribeItem.StartTime, out var startTime);
            TimeSpan.TryParse(transcribeItem.EndTime, out var endTime);
            Time = $"{startTime:mm\\:ss} - {endTime:mm\\:ss}";
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

        private void OnIsDirtyChanged()
        {
            IsDirtyChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
