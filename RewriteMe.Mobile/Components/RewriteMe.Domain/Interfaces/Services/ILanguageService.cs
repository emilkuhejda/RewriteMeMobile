using System.Globalization;
using System.Threading.Tasks;
using RewriteMe.Domain.Localization;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface ILanguageService
    {
        Task InitializeAsync();

        Task<LanguageInfo> GetLanguageInfo();

        Task ChangeUserLanguageAsync(CultureInfo language);
    }
}
