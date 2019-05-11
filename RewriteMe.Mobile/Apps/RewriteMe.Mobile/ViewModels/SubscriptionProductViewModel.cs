using System.Threading.Tasks;
using System.Windows.Input;
using RewriteMe.Domain.WebApi.Models;
using RewriteMe.Mobile.Commands;

namespace RewriteMe.Mobile.ViewModels
{
    public class SubscriptionProductViewModel
    {
        public SubscriptionProductViewModel(SubscriptionProduct subscriptionProduct)
        {
            SubscriptionProduct = subscriptionProduct;

            BuyCommand = new AsyncCommand(ExecuteBuyCommandAsync);
        }

        private SubscriptionProduct SubscriptionProduct { get; }

        public string Text => SubscriptionProduct.Id;

        public ICommand BuyCommand { get; }

        private async Task ExecuteBuyCommandAsync()
        { }
    }
}
