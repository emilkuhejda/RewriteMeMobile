﻿using RewriteMe.DataAccess.Entities;
using RewriteMe.Domain.WebApi;

namespace RewriteMe.DataAccess.DataAdapters
{
    public static class FileItemDataAdapter
    {
        public static FileItem ToFileItem(this FileItemEntity entity)
        {
            return new FileItem
            {
                Id = entity.Id,
                Name = entity.Name,
                FileName = entity.FileName,
                Language = entity.Language,
                IsPhoneCall = entity.IsPhoneCall,
                RecognitionStateString = entity.RecognitionState.ToString(),
                TotalTimeTicks = entity.TotalTime.Ticks,
                TranscriptionStartTimeTicks = entity.TranscriptionStartTime.Ticks,
                TranscriptionEndTimeTicks = entity.TranscriptionEndTime.Ticks,
                TranscribedTimeTicks = entity.TranscribedTime.Ticks,
                UploadStatus = entity.UploadStatus,
                UploadErrorCode = entity.UploadErrorCode,
                TranscribeErrorCode = entity.TranscribeErrorCode,
                DateCreated = entity.DateCreated,
                DateProcessedUtc = entity.DateProcessedUtc,
                DateUpdatedUtc = entity.DateUpdatedUtc
            };
        }

        public static FileItemEntity ToFileItemEntity(this FileItem fileItem)
        {
            return new FileItemEntity
            {
                Id = fileItem.Id,
                Name = fileItem.Name,
                FileName = fileItem.FileName,
                Language = fileItem.Language,
                IsPhoneCall = fileItem.IsPhoneCall,
                RecognitionState = fileItem.RecognitionState,
                TotalTime = fileItem.TotalTime,
                TranscriptionStartTime = fileItem.TranscriptionStartTime,
                TranscriptionEndTime = fileItem.TranscriptionEndTime,
                TranscribedTime = fileItem.TranscribedTime,
                UploadStatus = fileItem.UploadStatus,
                UploadErrorCode = fileItem.UploadErrorCode,
                TranscribeErrorCode = fileItem.TranscribeErrorCode,
                DateCreated = fileItem.DateCreated,
                DateProcessedUtc = fileItem.DateProcessedUtc,
                DateUpdatedUtc = fileItem.DateUpdatedUtc
            };
        }
    }
}
