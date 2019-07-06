using System.Collections.Generic;
using System.Threading.Tasks;
using RewriteMe.Domain.Transcription;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IRecordedItemService
    {
        Task<IEnumerable<RecordedItem>> GetAllAsync();
    }
}
