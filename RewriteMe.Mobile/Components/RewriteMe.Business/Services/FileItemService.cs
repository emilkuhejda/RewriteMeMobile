using System.Linq;
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

        public async Task SynchronizationAsync(int? applicationFileItemVersion)
        {
            var fileItemVersion = await _internalValueService.GetValueAsync(InternalValues.FileItemSynchronization).ConfigureAwait(false);
            if (applicationFileItemVersion.HasValue && applicationFileItemVersion.Value > fileItemVersion)
            {
                var httpRequestResult = await _rewriteMeWebService.GetFileItemsAsync(fileItemVersion).ConfigureAwait(false);
                if (httpRequestResult.State == HttpRequestState.Success)
                {
                    var fileItems = httpRequestResult.Payload.ToList();
                    var fileItem = fileItems.OrderByDescending(x => x.Version).FirstOrDefault();
                    var lastVersion = fileItem == null ? 0 : fileItem.Version;

                    await _fileItemRepository.UpdateAsync(fileItems).ConfigureAwait(false);
                    await _internalValueService.UpdateValueAsync(InternalValues.FileItemSynchronization, lastVersion);
                }
            }
        }
    }
}
