using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Interfaces.Configuration;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Localization;
using RewriteMe.Resources.Localization;

namespace RewriteMe.Mobile.Services
{
    public class LanguageService : ILanguageService
    {
        private readonly IInternalValueService _internalValueService;
        private readonly IApplicationSettings _applicationSettings;
        private readonly ILocalizer _localizer;

        public LanguageService(
            IInternalValueService internalValueService,
            IApplicationSettings applicationSettings,
            ILocalizer localizer)
        {
            _internalValueService = internalValueService;
            _applicationSettings = applicationSettings;
            _localizer = localizer;
        }

        public async Task InitializeAsync()
        {
            var languageInfo = await GetLanguageInfo().ConfigureAwait(false);
            _localizer.SetCultureInfo(languageInfo.GetCultureInfo());
        }

        public async Task<LanguageInfo> GetLanguageInfo()
        {
            var languageName = await GetLanguageName().ConfigureAwait(false);
            return Languages.All.FirstOrDefault(x => x.Culture == languageName) ?? Languages.English;
        }

        public async Task<string> GetLanguageName()
        {
            var language = await _internalValueService.GetValueAsync(InternalValues.LanguageSetting).ConfigureAwait(false);
            if (language != null)
                return language;

            var currentCulture = _localizer.GetCurrentCulture();
            var languageName = currentCulture.TwoLetterISOLanguageName;
            if (Languages.All.Any(x => x.Culture == languageName))
                return languageName;

            return _applicationSettings.DefaultLanguage;
        }

        public async Task ChangeUserLanguageAsync(CultureInfo cultureInfo)
        {
            await _internalValueService.UpdateValueAsync(InternalValues.LanguageSetting, cultureInfo.TwoLetterISOLanguageName).ConfigureAwait(false);
            _localizer.SetCultureInfo(cultureInfo);
        }
    }
}
