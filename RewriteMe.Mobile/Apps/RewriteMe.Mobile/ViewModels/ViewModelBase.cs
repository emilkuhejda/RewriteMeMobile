using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Navigation;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Extensions;

namespace RewriteMe.Mobile.ViewModels
{
    public class ViewModelBase : BindableBase, INavigatedAware
    {
        private string _title;
        private bool _hasTitleBar;
        private bool _canGoBack;

        public ViewModelBase(INavigationService navigationService)
        {
            HasTitleBar = true;
            NavigationService = navigationService;

            NavigateBackCommand = new AsyncCommand(ExecuteNavigateBackCommand/*, () => CanGoBack*/);
        }

        protected INavigationService NavigationService { get; }

        public IAsyncCommand NavigateBackCommand { get; }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public bool HasTitleBar
        {
            get => _hasTitleBar;
            protected set => SetProperty(ref _hasTitleBar, value);
        }

        public bool CanGoBack
        {
            get => _canGoBack;
            protected set => SetProperty(ref _canGoBack, value);
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
        }

        private async Task ExecuteNavigateBackCommand()
        {
            await NavigationService.GoBackWithoutAnimationAsync().ConfigureAwait(false);
        }
    }
}
