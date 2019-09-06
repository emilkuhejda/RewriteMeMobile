using System.Collections.Generic;
using System.Threading.Tasks;
using RewriteMe.Domain.Interfaces.Repositories;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.Business.Services
{
    public class InformationMessageService : IInformationMessageService
    {
        private readonly IInformationMessageRepository _informationMessageRepository;

        public InformationMessageService(IInformationMessageRepository informationMessageRepository)
        {
            _informationMessageRepository = informationMessageRepository;
        }

        public async Task<IEnumerable<InformationMessage>> GetAllAsync()
        {
            return await _informationMessageRepository.GetAllAsync().ConfigureAwait(false);
        }
    }
}
