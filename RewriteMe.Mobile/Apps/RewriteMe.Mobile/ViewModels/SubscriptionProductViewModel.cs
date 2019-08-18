using System;
using System.Threading.Tasks;
using System.Windows.Input;
using RewriteMe.Domain.Transcription;
using RewriteMe.Mobile.Commands;

namespace RewriteMe.Mobile.ViewModels
{
    public class SubscriptionProductViewModel
    {
        public SubscriptionProductViewModel(
            SubscriptionProduct subscriptionProduct,
            Func<string, Task> onBuyAction)
        {
            SubscriptionProduct = subscriptionProduct;
            OnBuyAction = onBuyAction;

            BuyCommand = new AsyncCommand(ExecuteBuyCommandAsync);
        }

        private SubscriptionProduct SubscriptionProduct { get; }

        public string IconKey => SubscriptionProduct.IconKey;

        public string Text => SubscriptionProduct.Text;

        public ICommand BuyCommand { get; }

        public Func<string, Task> OnBuyAction { get; }

        private async Task ExecuteBuyCommandAsync()
        {
            await OnBuyAction(SubscriptionProduct.Id).ConfigureAwait(false);
        }
    }
}
