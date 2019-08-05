﻿using RewriteMe.DataAccess.Entities;
using RewriteMe.Domain.Configuration;

namespace RewriteMe.DataAccess.DataAdapters
{
    public static class DeletedFileItemDataAdapter
    {
        public static DeletedFileItem ToDeletedFileItem(this DeletedFileItemEntity entity)
        {
            return new DeletedFileItem
            {
                Id = entity.Id,
                DeletedDate = entity.DeletedDate,
                RecognitionState = entity.RecognitionState,
                TotalTime = entity.TotalTime,
                TranscribedTime = entity.TranscribedTime
            };
        }

        public static DeletedFileItemEntity ToDeletedFileItemEntity(this DeletedFileItem deletedFileItem)
        {
            return new DeletedFileItemEntity
            {
                Id = deletedFileItem.Id,
                DeletedDate = deletedFileItem.DeletedDate,
                RecognitionState = deletedFileItem.RecognitionState,
                TotalTime = deletedFileItem.TotalTime,
                TranscribedTime = deletedFileItem.TranscribedTime
            };
        }
    }
}
