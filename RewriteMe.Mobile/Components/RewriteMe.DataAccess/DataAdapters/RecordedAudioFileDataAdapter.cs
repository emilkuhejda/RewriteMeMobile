using Newtonsoft.Json;
using RewriteMe.DataAccess.Entities;
using RewriteMe.Domain.Transcription;
using Xamarin.Cognitive.Speech;

namespace RewriteMe.DataAccess.DataAdapters
{
    public static class RecordedAudioFileDataAdapter
    {
        public static RecordedAudioFile ToRecordedAudioFile(this RecordedAudioFileEntity entity)
        {
            return new RecordedAudioFile
            {
                Id = entity.Id,
                RecordedItemId = entity.RecordedItemId,
                Path = entity.Path,
                Transcript = entity.Transcript,
                UserTranscript = entity.UserTranscript,
                RecognitionSpeechResult = JsonConvert.DeserializeObject<RecognitionSpeechResult>(entity.RecognitionSpeechResult),
                DateCreated = entity.DateCreated
            };
        }

        public static RecordedAudioFileEntity ToRecordedAudioFileEntity(this RecordedAudioFile recordedAudioFile)
        {
            return new RecordedAudioFileEntity
            {
                Id = recordedAudioFile.Id,
                RecordedItemId = recordedAudioFile.RecordedItemId,
                Path = recordedAudioFile.Path,
                Transcript = recordedAudioFile.Transcript,
                UserTranscript = recordedAudioFile.UserTranscript,
                RecognitionSpeechResult = JsonConvert.SerializeObject(recordedAudioFile.RecognitionSpeechResult),
                DateCreated = recordedAudioFile.DateCreated
            };
        }
    }
}
