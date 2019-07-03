﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RewriteMe.Domain.Transcription;

namespace RewriteMe.Domain.Interfaces.Repositories
{
    public interface IRecordedItemRepository
    {
        Task InsertAsync(RecordedItem recordedItem);

        Task<IEnumerable<RecordedItem>> GetAllAsync();

        Task<RecordedItem> GetAsync(Guid recordedItemId);
    }
}
