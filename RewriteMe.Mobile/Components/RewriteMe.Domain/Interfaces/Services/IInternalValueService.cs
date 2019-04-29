using System.Threading.Tasks;
using RewriteMe.Domain.Configuration;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IInternalValueService
    {
        Task<T> GetValueAsync<T>(InternalValue<T> internalValue);

        Task UpdateValueAsync<T>(InternalValue<T> internalValue, T value);
    }
}
