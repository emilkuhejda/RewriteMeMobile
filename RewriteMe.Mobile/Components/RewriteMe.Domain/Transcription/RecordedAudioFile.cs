﻿using System;
using Xamarin.Cognitive.Speech;

namespace RewriteMe.Domain.Transcription
{
    public class RecordedAudioFile
    {
        public Guid Id { get; set; }

        public Guid RecordedItemId { get; set; }

        public string Transcript { get; set; }

        public string UserTranscript { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public TimeSpan TotalTime { get; set; }

        public string TimeRange
        {
            get
            {
                if (StartTime.TotalHours >= 1)
                {
                    return $"{StartTime:hh\\:mm\\:ss} - {EndTime:hh\\:mm\\:ss}";
                }

                return $"{StartTime:mm\\:ss} - {EndTime:mm\\:ss}";
            }
        }

        public byte[] Source { get; set; }

        public RecognitionSpeechResult RecognitionSpeechResult { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
