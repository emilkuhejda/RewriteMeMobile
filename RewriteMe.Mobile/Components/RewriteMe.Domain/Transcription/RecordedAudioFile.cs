﻿using System;
using Xamarin.Cognitive.Speech;

namespace RewriteMe.Domain.Transcription
{
    public class RecordedAudioFile
    {
        public Guid Id { get; set; }

        public Guid RecordedItemId { get; set; }

        public string Path { get; set; }

        public string Transcript { get; set; }

        public RecognitionSpeechResult RecognitionSpeechResult { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
