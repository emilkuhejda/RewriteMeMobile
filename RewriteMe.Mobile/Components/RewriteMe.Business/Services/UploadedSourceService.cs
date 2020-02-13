using System;
using System.Threading.Tasks;
using RewriteMe.Domain.Interfaces.Repositories;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Upload;

namespace RewriteMe.Business.Services
{
    public class UploadedSourceService : IUploadedSourceService
    {
        private readonly IUploadedSourceRepository _uploadedSourceRepository;

        public UploadedSourceService(IUploadedSourceRepository uploadedSourceRepository)
        {
            _uploadedSourceRepository = uploadedSourceRepository;
        }

        public async Task<UploadedSource> GetFirstAsync()
        {
            return await _uploadedSourceRepository.GetFirstAsync().ConfigureAwait(false);
        }

        public async Task AddAsync(UploadedSource uploadedSource)
        {
            await _uploadedSourceRepository.AddAsync(uploadedSource).ConfigureAwait(false);
        }

        public async Task DeleteAsync(Guid uploadedSourceId)
        {
            await _uploadedSourceRepository.DeleteAsync(uploadedSourceId).ConfigureAwait(false);
        }
    }
}
