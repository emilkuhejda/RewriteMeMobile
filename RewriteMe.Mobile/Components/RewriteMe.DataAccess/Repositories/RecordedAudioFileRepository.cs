using RewriteMe.DataAccess.Providers;
using RewriteMe.Domain.Interfaces.Repositories;

namespace RewriteMe.DataAccess.Repositories
{
    public class RecordedAudioFileRepository : IRecordedAudioFileRepository
    {
        private readonly IAppDbContextProvider _contextProvider;

        public RecordedAudioFileRepository(IAppDbContextProvider contextProvider)
        {
            _contextProvider = contextProvider;
        }
    }
}
