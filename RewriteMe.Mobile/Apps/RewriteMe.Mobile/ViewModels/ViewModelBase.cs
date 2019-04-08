using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Mvvm;
using Prism.Navigation;
using RewriteMe.Common.Utils;
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
            NavigationService = navigationService;
            OperationScope = new AsyncOperationScope();

            HasTitleBar = true;

            NavigateBackCommand = new AsyncCommand(ExecuteNavigateBackCommand, () => CanGoBack);
        }

        protected INavigationService NavigationService { get; }

        public AsyncOperationScope OperationScope { get; }

        public ICommand NavigateBackCommand { get; }

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

        public async void OnNavigatedTo(INavigationParameters parameters)
        {
            await LoadDataAsync(parameters).ConfigureAwait(false);
        }

        protected virtual async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            await Task.CompletedTask.ConfigureAwait(false);
        }

        private async Task ExecuteNavigateBackCommand()
        {
            await NavigationService.GoBackWithoutAnimationAsync().ConfigureAwait(false);
        }
    }
}
