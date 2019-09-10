using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface ITranscribeItemService
    {
        Task SynchronizationAsync(DateTime applicationUpdateDate);

        Task AudioSourcesSynchronizationAsync(CancellationToken cancellationToken);

        Task<IEnumerable<TranscribeItem>> GetAllAsync(Guid fileItemId);

        Task SaveAndSendAsync(IEnumerable<TranscribeItem> transcribeItems);

        Task SendPendingAsync();
    }
}
