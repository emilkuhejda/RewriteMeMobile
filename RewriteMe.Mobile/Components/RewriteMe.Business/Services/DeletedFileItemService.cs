using System.Linq;
using System.Threading.Tasks;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.Interfaces.Repositories;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.Business.Services
{
    public class DeletedFileItemService : IDeletedFileItemService
    {
        private readonly IDeletedFileItemRepository _deletedFileItemRepository;
        private readonly IRewriteMeWebService _rewriteMeWebService;

        public DeletedFileItemService(
            IDeletedFileItemRepository deletedFileItemRepository,
            IRewriteMeWebService rewriteMeWebService)
        {
            _deletedFileItemRepository = deletedFileItemRepository;
            _rewriteMeWebService = rewriteMeWebService;
        }

        public async Task InsertAsync(DeletedFileItem deletedFileItem)
        {
            await _deletedFileItemRepository.InsertAsync(deletedFileItem).ConfigureAwait(false);
        }

        public async Task SendPendingAsync()
        {
            var pendingDeletedFileItems = await _deletedFileItemRepository.GetAllAsync().ConfigureAwait(false);
            var pendingFileItems = pendingDeletedFileItems.ToList();
            if (pendingFileItems.Any())
                return;

            var httpRequestResult = await _rewriteMeWebService.DeleteAllFileItemAsync(pendingFileItems).ConfigureAwait(false);
            if (httpRequestResult.State == HttpRequestState.Success)
            {
                await _deletedFileItemRepository.ClearAsync().ConfigureAwait(false);
            }
        }
    }
}
