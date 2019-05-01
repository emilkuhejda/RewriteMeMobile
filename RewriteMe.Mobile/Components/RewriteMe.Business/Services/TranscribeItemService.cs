using System;
using System.Collections.Generic;
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
                    await _transcribeItemRepository.UpdateAsync(httpRequestResult.Payload).ConfigureAwait(false);
                    await _internalValueService.UpdateValueAsync(InternalValues.TranscribeItemSynchronization, DateTime.UtcNow);
                }
            }
        }

        public async Task<IEnumerable<TranscribeItem>> GetAllAsync(Guid fileItemId)
        {
            return await _transcribeItemRepository.GetAllAsync(fileItemId).ConfigureAwait(false);
        }
    }
}
