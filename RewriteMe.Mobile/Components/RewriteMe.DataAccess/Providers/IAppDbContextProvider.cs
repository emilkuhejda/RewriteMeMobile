using System.Threading.Tasks;

namespace RewriteMe.DataAccess.Providers
{
    public interface IAppDbContextProvider
    {
        IAppDbContext Context { get; }

        Task CloseAsync();
    }
}
