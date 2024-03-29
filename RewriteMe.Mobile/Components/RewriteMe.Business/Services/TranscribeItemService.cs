﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using RewriteMe.Business.Extensions;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.Interfaces.Repositories;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.WebApi;
using RewriteMe.Logging.Extensions;
using RewriteMe.Logging.Interfaces;

namespace RewriteMe.Business.Services
{
    public class TranscribeItemService : ITranscribeItemService
    {
        private readonly IInternalValueService _internalValueService;
        private readonly IRewriteMeWebService _rewriteMeWebService;
        private readonly ITranscribeItemRepository _transcribeItemRepository;
        private readonly ILogger _logger;

        public TranscribeItemService(
            IInternalValueService internalValueService,
            IRewriteMeWebService rewriteMeWebService,
            ITranscribeItemRepository transcribeItemRepository,
            ILoggerFactory loggerFactory)
        {
            _internalValueService = internalValueService;
            _rewriteMeWebService = rewriteMeWebService;
            _transcribeItemRepository = transcribeItemRepository;
            _logger = loggerFactory.CreateLogger(typeof(TranscribeItemService));
        }

        public async Task SynchronizationAsync(DateTime applicationUpdateDate)
        {
            var lastTranscribeItemSynchronizationTicks = await _internalValueService.GetValueAsync(InternalValues.TranscribeItemSynchronizationTicks).ConfigureAwait(false);
            var lastTranscribeItemSynchronization = new DateTime(lastTranscribeItemSynchronizationTicks);
            _logger.Debug($"Update transcribe items with timestamp '{lastTranscribeItemSynchronization.ToString("d", CultureInfo.InvariantCulture)}'.");

            if (applicationUpdateDate > lastTranscribeItemSynchronization)
            {
                var httpRequestResult = await _rewriteMeWebService.GetTranscribeItemsAllAsync(lastTranscribeItemSynchronization).ConfigureAwait(false);
                if (httpRequestResult.State == HttpRequestState.Success)
                {
                    var transcribeItems = httpRequestResult.Payload.ToList();
                    _logger.Info($"Web server returned {transcribeItems.Count} items for synchronization.");

                    await _transcribeItemRepository.InsertOrReplaceAllAsync(transcribeItems).ConfigureAwait(false);
                    await _internalValueService.UpdateValueAsync(InternalValues.TranscribeItemSynchronizationTicks, applicationUpdateDate.Ticks).ConfigureAwait(false);
                }
            }
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

        public async Task SendPendingAsync()
        {
            var pendingTranscribeItems = await _transcribeItemRepository.GetPendingAsync().ConfigureAwait(false);
            var transcribeItems = pendingTranscribeItems.ToList();
            if (!transcribeItems.Any())
                return;

            _logger.Info($"Send pending transcribe items {transcribeItems.Count} to server.");

            SendAllAsync(transcribeItems);
        }

        private void SendAllAsync(IEnumerable<TranscribeItem> transcribeItems)
        {
            foreach (var transcribeItem in transcribeItems)
            {
                SendAsync(transcribeItem).FireAndForget();
            }
        }

        private async Task SendAsync(TranscribeItem transcribeItem)
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
