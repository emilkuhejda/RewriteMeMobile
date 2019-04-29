using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.Domain.Interfaces.Repositories
{
    public interface ITranscribeItemRepository
    {
        Task<IEnumerable<TranscribeItem>> GetAllAsync(Guid fileItemId);

        Task UpdateAsync(IEnumerable<TranscribeItem> transcribeItems);
    }
}
