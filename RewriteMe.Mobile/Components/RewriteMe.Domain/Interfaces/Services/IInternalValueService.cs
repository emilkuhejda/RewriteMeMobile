using System.Threading.Tasks;
using RewriteMe.Domain.Configuration;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IInternalValueService
    {
        Task<T> GetValue<T>(InternalValue<T> internalValue);

        Task UpdateValue<T>(InternalValue<T> internalValue, T value);
    }
}
