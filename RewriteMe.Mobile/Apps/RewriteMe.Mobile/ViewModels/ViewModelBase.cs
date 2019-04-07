using Prism.Mvvm;
using Prism.Navigation;

namespace RewriteMe.Mobile.ViewModels
{
    public class ViewModelBase : BindableBase, INavigatedAware
    {
        public void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
        }
    }
}
