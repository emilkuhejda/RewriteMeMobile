using System.Threading.Tasks;

namespace RewriteMe.DataAccess
{
    public interface IStorageInitializer
    {
        Task InitializeAsync();
    }
}
