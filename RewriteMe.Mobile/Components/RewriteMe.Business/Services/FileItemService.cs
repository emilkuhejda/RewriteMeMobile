using System;
using System.Threading.Tasks;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.Interfaces.Repositories;
using RewriteMe.Domain.Interfaces.Services;

namespace RewriteMe.Business.Services
{
    public class FileItemService : IFileItemService
    {
        private readonly IInternalValueService _internalValueService;
        private readonly IFileItemRepository _fileItemRepository;
        private readonly IRewriteMeWebService _rewriteMeWebService;

        public FileItemService(
            IInternalValueService internalValueService,
            IFileItemRepository fileItemRepository,
            IRewriteMeWebService rewriteMeWebService)
        {
            _internalValueService = internalValueService;
            _fileItemRepository = fileItemRepository;
            _rewriteMeWebService = rewriteMeWebService;
        }

        public async Task SynchronizationAsync(DateTime applicationUpdateDate)
        {
            var lastFileItemSynchronization = await _internalValueService.GetValueAsync(InternalValues.FileItemSynchronization).ConfigureAwait(false);
            if (applicationUpdateDate >= lastFileItemSynchronization)
            {
                var httpRequestResult = await _rewriteMeWebService.GetFileItemsAsync(lastFileItemSynchronization).ConfigureAwait(false);
                if (httpRequestResult.State == HttpRequestState.Success)
                {
                    await _fileItemRepository.UpdateAsync(httpRequestResult.Payload).ConfigureAwait(false);
                    await _internalValueService.UpdateValueAsync(InternalValues.FileItemSynchronization, DateTime.UtcNow);
                }
            }
        }
    }
}
