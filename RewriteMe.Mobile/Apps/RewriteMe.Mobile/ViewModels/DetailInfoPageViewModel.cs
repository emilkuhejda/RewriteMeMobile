using System.Linq;
using System.Threading.Tasks;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Extensions;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.WebApi.Models;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Extensions;
using Xamarin.Forms;

namespace RewriteMe.Mobile.ViewModels
{
    public class DetailInfoPageViewModel : ViewModelBase
    {
        private readonly ILanguageService _languageService;
        private readonly IInformationMessageService _informationMessageService;

        private string _title;
        private HtmlWebViewSource _description;

        public DetailInfoPageViewModel(
            ILanguageService languageService,
            IInformationMessageService informationMessageService,
            IUserSessionService userSessionService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(userSessionService, dialogService, navigationService, loggerFactory)
        {
            _languageService = languageService;
            _informationMessageService = informationMessageService;

            CanGoBack = true;
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public HtmlWebViewSource Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                var languageInfo = await _languageService.GetLanguageInfo().ConfigureAwait(false);
                var informationMessage = navigationParameters.GetValue<InformationMessage>();
                var languageVersion = informationMessage?.LanguageVersions.FirstOrDefault(x => x.Language == languageInfo.ToLanguage());
                if (languageVersion == null)
                    return;

                Title = languageVersion.Title;
                Description = new HtmlWebViewSource { Html = languageVersion.Description };

                if (!informationMessage.WasOpened)
                {
                    await _informationMessageService.MarkAsOpenedAsync(informationMessage).ConfigureAwait(false);
                }
            }
        }
    }
}
