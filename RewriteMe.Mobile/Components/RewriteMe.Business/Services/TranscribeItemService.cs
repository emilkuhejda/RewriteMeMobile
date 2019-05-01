using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.Interfaces.Repositories;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.Business.Services
{
    public class TranscribeItemService : ITranscribeItemService
    {
        private readonly IInternalValueService _internalValueService;
        private readonly ITranscribeItemRepository _transcribeItemRepository;
        private readonly IRewriteMeWebService _rewriteMeWebService;

        public TranscribeItemService(
            IInternalValueService internalValueService,
            ITranscribeItemRepository transcribeItemRepository,
            IRewriteMeWebService rewriteMeWebService)
        {
            _internalValueService = internalValueService;
            _transcribeItemRepository = transcribeItemRepository;
            _rewriteMeWebService = rewriteMeWebService;
        }

        public async Task SynchronizationAsync(DateTime applicationUpdateDate)
        {
            var lastTranscribeItemSynchronization = await _internalValueService.GetValueAsync(InternalValues.TranscribeItemSynchronization).ConfigureAwait(false);
            if (applicationUpdateDate >= lastTranscribeItemSynchronization)
            {
                var httpRequestResult = await _rewriteMeWebService.GetTranscribeItemsAllAsync(lastTranscribeItemSynchronization).ConfigureAwait(false);
                if (httpRequestResult.State == HttpRequestState.Success)
                {
                    await _transcribeItemRepository.InsertOrReplaceAllAsync(httpRequestResult.Payload).ConfigureAwait(false);
                    await _internalValueService.UpdateValueAsync(InternalValues.TranscribeItemSynchronization, DateTime.UtcNow);
                }
            }

            SendPendingTranscribeItemsAsync();
        }

        private async void SendPendingTranscribeItemsAsync()
        {
            var pendingTranscribeItems = await _transcribeItemRepository.GetPendingAsync().ConfigureAwait(false);
            var transcribeItems = pendingTranscribeItems.ToList();
            if (!transcribeItems.Any())
                return;

            SendAllAsync(transcribeItems);
        }

        public async Task<IEnumerable<TranscribeItem>> GetAllAsync(Guid fileItemId)
        {
            return await _transcribeItemRepository.GetAllAsync(fileItemId).ConfigureAwait(false);
        }

        public async Task SaveAndSendAsync(IEnumerable<TranscribeItem> transcribeItems)
        {
            var items = transcribeItems.ToList();
            await _transcribeItemRepository.UpdateAllAsync(items).ConfigureAwait(false);

            SendAllAsync(items);
        }

        private void SendAllAsync(IEnumerable<TranscribeItem> transcribeItems)
        {
            foreach (var transcribeItem in transcribeItems)
            {
                Send(transcribeItem);
            }
        }

        private async void Send(TranscribeItem transcribeItem)
        {
            var transcribeItemId = transcribeItem.Id ?? Guid.Empty;
            var httpRequestResult = await _rewriteMeWebService.UpdateUserTranscriptAsync(transcribeItemId, transcribeItem.UserTranscript).ConfigureAwait(false);
            if (httpRequestResult.State != HttpRequestState.Success)
            {
                transcribeItem.IsPendingSynchronization = true;
                await _transcribeItemRepository.UpdateAsync(transcribeItem).ConfigureAwait(false);
            }
        }
    }
}
