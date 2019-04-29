using System;
using System.Threading.Tasks;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface ITranscribeItemService
    {
        Task SynchronizationAsync(DateTime applicationUpdateDate);
    }
}
