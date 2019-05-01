using System;
using System.Linq;
using Prism.Mvvm;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.Mobile.ViewModels
{
    public class TranscribeItemViewModel : BindableBase
    {
        private string _time;
        private string _userTranscript;

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

        public string Time
        {
            get => _time;
            set => SetProperty(ref _time, value);
        }

        public string UserTranscript
        {
            get => _userTranscript;
            set
            {
                if (SetProperty(ref _userTranscript, value))
                {
                    TranscribeItem.UserTranscript = _userTranscript;
                }
            }
        }
    }
}
