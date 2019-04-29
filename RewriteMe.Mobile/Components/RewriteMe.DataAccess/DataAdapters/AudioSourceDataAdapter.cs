using RewriteMe.DataAccess.Entities;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.DataAccess.DataAdapters
{
    public static class AudioSourceDataAdapter
    {
        public static AudioSource ToAudioSource(this AudioSourceEntity entity)
        {
            return new AudioSource
            {
                Id = entity.Id,
                FileItemId = entity.FileItemId,
                ContentType = entity.ContentType,
                Version = entity.Version
            };
        }

        public static AudioSourceEntity ToAudioSourceEntity(this AudioSource audioSource)
        {
            return new AudioSourceEntity
            {
                Id = audioSource.Id.GetValueOrDefault(),
                FileItemId = audioSource.FileItemId.GetValueOrDefault(),
                ContentType = audioSource.ContentType,
                Version = audioSource.Version.GetValueOrDefault()
            };
        }
    }
}
