using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.Navigation;

namespace RewriteMe.Mobile.ViewModels
{
    public class LoadingPageViewModel : ViewModelBase
    {
        private readonly IRewriteMeWebService _rewriteMeWebService;
        private readonly IInternalValueService _internalValueService;

        private string _progressText;

        public LoadingPageViewModel(
            IRewriteMeWebService rewriteMeWebService,
            IInternalValueService internalValueService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(dialogService, navigationService, loggerFactory)
        {
            _rewriteMeWebService = rewriteMeWebService;
            _internalValueService = internalValueService;

            HasTitleBar = false;

            ReloadCommand = new AsyncCommand(ExecuteReloadCommandAsync);
        }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                var isUserRegistrationSuccess = await _internalValueService.GetValue(InternalValues.IsUserRegistrationSuccess).ConfigureAwait(false);

                //await NavigationService.NavigateWithoutAnimationAsync($"/{Pages.Navigation}/{Pages.Overview}").ConfigureAwait(false);
            }
        }

        public ICommand ReloadCommand { get; }

        public string ProgressText
        {
            get => _progressText;
            set => SetProperty(ref _progressText, value);
        }

        private async Task ExecuteReloadCommandAsync()
        {
            await LoadDataAsync(null).ConfigureAwait(false);
        }
    }
}
