using System;
using System.Threading.Tasks;
using System.Windows.Input;
using RewriteMe.Mobile.Commands;

namespace RewriteMe.Mobile.ViewModels
{
    public class SubscriptionProductViewModel
    {
        public SubscriptionProductViewModel(string productId, Func<string, Task> onBuyAction)
        {
            ProductId = productId;
            OnBuyAction = onBuyAction;

            BuyCommand = new AsyncCommand(ExecuteBuyCommandAsync);
        }

        public string ProductId { get; }

        public string Title { get; set; }

        public string Price { get; set; }

        public string Description { get; set; }

        public ICommand BuyCommand { get; }

        public Func<string, Task> OnBuyAction { get; }

        private async Task ExecuteBuyCommandAsync()
        {
            await OnBuyAction(ProductId).ConfigureAwait(false);
        }
    }
}
