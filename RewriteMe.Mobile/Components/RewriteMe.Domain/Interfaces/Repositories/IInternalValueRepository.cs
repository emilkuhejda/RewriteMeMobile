using System.Threading.Tasks;

namespace RewriteMe.Domain.Interfaces.Repositories
{
    public interface IInternalValueRepository
    {
        Task<string> GetValue(string key);

        Task UpdateValue(string key, string value);
    }
}
