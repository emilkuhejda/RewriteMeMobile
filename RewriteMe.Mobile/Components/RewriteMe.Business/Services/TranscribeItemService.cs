using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.Interfaces.Repositories;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.WebApi.Models;
using RewriteMe.Logging.Extensions;
using RewriteMe.Logging.Interfaces;

namespace RewriteMe.Business.Services
{
    public class TranscribeItemService : ITranscribeItemService
    {
        private readonly IInternalValueService _internalValueService;
        private readonly ITranscribeItemRepository _transcribeItemRepository;
        private readonly IRewriteMeWebService _rewriteMeWebService;
        private readonly ILogger _logger;

        public TranscribeItemService(
            IInternalValueService internalValueService,
            ITranscribeItemRepository transcribeItemRepository,
            IRewriteMeWebService rewriteMeWebService,
            ILoggerFactory loggerFactory)
        {
            _internalValueService = internalValueService;
            _transcribeItemRepository = transcribeItemRepository;
            _rewriteMeWebService = rewriteMeWebService;
            _logger = loggerFactory.CreateLogger(typeof(TranscribeItemService));
        }

        public async Task SynchronizationAsync(DateTime applicationUpdateDate)
        {
            var lastTranscribeItemSynchronizationTicks = await _internalValueService.GetValueAsync(InternalValues.TranscribeItemSynchronizationTicks).ConfigureAwait(false);
            var lastTranscribeItemSynchronization = new DateTime(lastTranscribeItemSynchronizationTicks);
            _logger.Debug($"Update transcribe items with timestamp '{lastTranscribeItemSynchronization.ToString("d", CultureInfo.InvariantCulture)}'.");

            if (applicationUpdateDate >= lastTranscribeItemSynchronization)
            {
                var httpRequestResult = await _rewriteMeWebService.GetTranscribeItemsAllAsync(lastTranscribeItemSynchronization).ConfigureAwait(false);
                if (httpRequestResult.State == HttpRequestState.Success)
                {
                    var transcribeItems = httpRequestResult.Payload.ToList();
                    _logger.Info($"Web server returned {transcribeItems.Count} items for synchronization.");

                    await _transcribeItemRepository.InsertOrReplaceAllAsync(transcribeItems).ConfigureAwait(false);
                    await _internalValueService.UpdateValueAsync(InternalValues.TranscribeItemSynchronizationTicks, DateTime.UtcNow.Ticks).ConfigureAwait(false);
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
            var httpRequestResult = await _rewriteMeWebService.UpdateUserTranscriptAsync(transcribeItem.Id, transcribeItem.UserTranscript).ConfigureAwait(false);
            if (httpRequestResult.State != HttpRequestState.Success)
            {
                transcribeItem.IsPendingSynchronization = true;
                await _transcribeItemRepository.UpdateAsync(transcribeItem).ConfigureAwait(false);
            }
        }
    }
}
