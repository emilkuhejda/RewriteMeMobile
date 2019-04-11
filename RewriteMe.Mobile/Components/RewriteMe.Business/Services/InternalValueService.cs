using System;
using System.Threading.Tasks;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Interfaces.Repositories;
using RewriteMe.Domain.Interfaces.Services;

namespace RewriteMe.Business.Services
{
    public class InternalValueService : IInternalValueService
    {
        private readonly IInternalValueRepository _internalValueRepository;

        public InternalValueService(IInternalValueRepository internalValueRepository)
        {
            _internalValueRepository = internalValueRepository;
        }

        public async Task<T> GetValue<T>(InternalValue<T> internalValue)
        {
            var key = internalValue.Key;
            var result = await _internalValueRepository.GetValue(key).ConfigureAwait(false);
            if (result == null)
                return internalValue.DefaultValue;

            return ParseResult(result, internalValue.DefaultValue);
        }

        private T ParseResult<T>(string value, T defaultValue)
        {
            try
            {
                var result = (T)Convert.ChangeType(value, typeof(T));
                return result;
            }
            catch (NotSupportedException)
            {
                return defaultValue;
            }
        }

        public async Task UpdateValue<T>(InternalValue<T> internalValue, T value)
        {
            var key = internalValue.Key;
            var entityValue = Convert.ToString(value);

            await _internalValueRepository.UpdateValue(key, entityValue).ConfigureAwait(false);
        }
    }
}
