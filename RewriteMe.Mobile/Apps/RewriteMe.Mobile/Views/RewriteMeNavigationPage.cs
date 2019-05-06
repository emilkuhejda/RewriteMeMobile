using System;
using Xamarin.Forms;

namespace RewriteMe.Mobile.Views
{
    public class RewriteMeNavigationPage : NavigationPage
    {
        public RewriteMeNavigationPage()
        {
            Popped += HandlePopped;
        }

        private void HandlePopped(object sender, NavigationEventArgs e)
        {
            var disposable = e.Page.BindingContext as IDisposable;
            disposable?.Dispose();
        }
    }
}
