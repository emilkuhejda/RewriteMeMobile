using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.DataAccess.Transcription;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.Navigation;
using RewriteMe.Mobile.Navigation.Parameters;
using RewriteMe.Mobile.Transcription;
using RewriteMe.Resources.Localization;

namespace RewriteMe.Mobile.ViewModels
{
    public class CreatePageViewModel : ViewModelBase
    {
        private readonly IRewriteMeWebService _rewriteMeWebService;

        private string _fileName;
        private SupportedLanguage _selectedLanguage;

        public CreatePageViewModel(
            IRewriteMeWebService rewriteMeWebService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(dialogService, navigationService, loggerFactory)
        {
            _rewriteMeWebService = rewriteMeWebService;

            CanGoBack = true;

            NavigateToLanguageCommand = new AsyncCommand(ExecuteNavigateToLanguageCommandAsync);
            SaveCommand = new AsyncCommand(ExecuteSaveCommandAsync);
        }

        public string FileName
        {
            get => _fileName;
            set => SetProperty(ref _fileName, value);
        }

        public SupportedLanguage SelectedLanguage
        {
            get => _selectedLanguage;
            set => SetProperty(ref _selectedLanguage, value);
        }

        public ICommand NavigateToLanguageCommand { get; }

        public ICommand SaveCommand { get; }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                if (navigationParameters.GetNavigationMode() == NavigationMode.Back)
                {
                    var dropDownListViewModel = navigationParameters.GetValue<DropDownListViewModel>();
                    HandleSelectionAsync(dropDownListViewModel);
                }

                await Task.CompletedTask.ConfigureAwait(false);
            }
        }

        private void HandleSelectionAsync(DropDownListViewModel dropDownListViewModel)
        {
            if (dropDownListViewModel == null)
                return;

            switch (dropDownListViewModel.Type)
            {
                case nameof(SelectedLanguage):
                    SelectedLanguage = (SupportedLanguage)dropDownListViewModel.Value;
                    break;
                default:
                    throw new NotSupportedException(nameof(SelectedLanguage));
            }
        }

        private async Task ExecuteNavigateToLanguageCommandAsync()
        {
            var languages = SupportedLanguages.All.Select(x => new DropDownListViewModel
            {
                Text = x.Title,
                Value = x,
                Type = nameof(SelectedLanguage),
                IsSelected = SelectedLanguage != null && x.Culture == SelectedLanguage.Culture
            });

            var navigationParameters = new NavigationParameters();
            var parameters = new DropDownListNavigationParameters(Loc.Text(TranslationKeys.Languages), languages);
            navigationParameters.Add<DropDownListNavigationParameters>(parameters);

            await NavigationService.NavigateWithoutAnimationAsync(Pages.DropDownListPage, navigationParameters).ConfigureAwait(false);
        }

        private async Task ExecuteSaveCommandAsync()
        {
        }
    }
}
