using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plugin.InAppBilling;
using Plugin.InAppBilling.Abstractions;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Exceptions;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Logging.Extensions;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.Transcription;
using RewriteMe.Resources.Localization;
using Xamarin.Forms;

namespace RewriteMe.Mobile.ViewModels
{
    public class UserSubscriptionsPageViewModel : ViewModelBase
    {
        private readonly IUserSessionService _userSessionService;
        private readonly IUserSubscriptionService _userSubscriptionService;
        private readonly IBillingPurchaseService _billingPurchaseService;

        private IList<SubscriptionProductViewModel> _products;

        public UserSubscriptionsPageViewModel(
            IUserSessionService userSessionService,
            IUserSubscriptionService userSubscriptionService,
            IBillingPurchaseService billingPurchaseService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(dialogService, navigationService, loggerFactory)
        {
            _userSessionService = userSessionService;
            _userSubscriptionService = userSubscriptionService;
            _billingPurchaseService = billingPurchaseService;

            CanGoBack = true;
        }

        public IList<SubscriptionProductViewModel> Products
        {
            get => _products;
            private set => SetProperty(ref _products, value);
        }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                Products = SubscriptionProducts.All
                    .Select(x => new SubscriptionProductViewModel(x, OnBuyAction))
                    .ToList();

                await Task.CompletedTask.ConfigureAwait(false);
            }
        }

        public async Task OnBuyAction(string productId)
        {
            try
            {
                if (!CrossInAppBilling.IsSupported)
                    throw new InAppBillingNotSupportedException();

                Logger.Info($"Start purchasing product '{productId}'.");

                var payload = Guid.NewGuid();
                var billing = CrossInAppBilling.Current;
                var connected = await billing.ConnectAsync().ConfigureAwait(false);
                if (!connected)
                    throw new AppStoreNotConnectedException();

                var purchase = await billing
                    .PurchaseAsync(productId, ItemType.InAppPurchase, payload.ToString())
                    .ConfigureAwait(false);

                if (purchase != null && purchase.State == PurchaseState.Purchased)
                {
                    if (purchase.Payload == payload.ToString())
                        throw new PurchasePayloadNotValidException(nameof(payload));

                    InAppBillingPurchase billingPurchase = null;
                    if (Device.RuntimePlatform == Device.iOS)
                    {
                        Logger.Info($"Product '{productId}' was purchased.");

                        billingPurchase = purchase;
                    }
                    else
                    {
                        var consumedItem = await billing
                            .ConsumePurchaseAsync(purchase.ProductId, purchase.PurchaseToken).ConfigureAwait(false);
                        if (consumedItem != null)
                        {
                            Logger.Info($"Product '{productId}' was purchased.");

                            billingPurchase = consumedItem;
                        }
                    }

                    if (billingPurchase != null)
                    {
                        await SendBillingPurchaseAsync(productId, billingPurchase).ConfigureAwait(false);
                    }
                    else
                    {
                        throw new PurchaseWasNotProcessedException();
                    }
                }
                else
                {
                    throw new PurchaseWasNotProcessedException();
                }
            }
            catch (InAppBillingNotSupportedException ex)
            {
                Logger.Warning($"In-App Purchases is not supported in the device. {ex}");
                await DialogService.AlertAsync(Loc.Text(TranslationKeys.InAppBillingIsNotSupportedErrorMessage)).ConfigureAwait(false);
            }
            catch (AppStoreNotConnectedException ex)
            {
                Logger.Error($"App store is not connected. {ex}");
                await DialogService.AlertAsync(Loc.Text(TranslationKeys.AppStoreUnavailableErrorMessage)).ConfigureAwait(false);
            }
            catch (PurchasePayloadNotValidException ex)
            {
                Logger.Error($"Purchase payload is not valid. {ex}");

                var result = await DialogService.ConfirmAsync(
                    Loc.Text(TranslationKeys.PurchaseProcessedIncorrectlyErrorMessage),
                    okText: Loc.Text(TranslationKeys.SendEmail),
                    cancelText: Loc.Text(TranslationKeys.Ok)).ConfigureAwait(false);

                if (result)
                {
                    // TODO
                }
            }
            catch (PurchaseWasNotProcessedException ex)
            {
                Logger.Warning($"Product '{productId}' was not purchased. {ex}");

                await DialogService.AlertAsync(Loc.Text(TranslationKeys.PurchaseWasNotProcessedErrorMessage)).ConfigureAwait(false);
            }
            catch (ErrorRequestException ex)
            {
                Logger.Error($"Exception during registration of purchase billing. {ex}");

                var result = await DialogService.ConfirmAsync(
                    Loc.Text(TranslationKeys.RegistrationPurchaseBillingErrorMessage),
                    okText: Loc.Text(TranslationKeys.SendEmail),
                    cancelText: Loc.Text(TranslationKeys.Ok)).ConfigureAwait(false);

                if (result)
                {
                    // TODO
                }
            }
            catch (InAppBillingPurchaseException ex)
            {
                Logger.Error($"Exception during purchasing process. {ex}");

                var message = Loc.Text(TranslationKeys.AppStoreUnavailableErrorMessage);
                switch (ex.PurchaseError)
                {
                    case PurchaseError.AppStoreUnavailable:
                        message = Loc.Text(TranslationKeys.AppStoreUnavailableErrorMessage);
                        break;
                    case PurchaseError.BillingUnavailable:
                        message = Loc.Text(TranslationKeys.BillingUnavailableErrorMessage);
                        break;
                    case PurchaseError.PaymentInvalid:
                        message = Loc.Text(TranslationKeys.PaymentInvalidErrorMessage);
                        break;
                    case PurchaseError.PaymentNotAllowed:
                        message = Loc.Text(TranslationKeys.PaymentNotAllowedErrorMessage);
                        break;
                }

                await DialogService.AlertAsync(message).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.Error($"Exception during purchasing process. {ex}");

                await DialogService.AlertAsync(Loc.Text(TranslationKeys.PurchaseWasNotProcessedErrorMessage)).ConfigureAwait(false);
            }
            finally
            {
                await CrossInAppBilling.Current.DisconnectAsync().ConfigureAwait(false);
            }
        }

        private async Task SendBillingPurchaseAsync(string productId, InAppBillingPurchase purchase)
        {
            var userId = await _userSessionService.GetUserIdAsync().ConfigureAwait(false);
            var userSubscription = await _billingPurchaseService.SendBillingPurchaseAsync(purchase.ToBillingPurchase(userId)).ConfigureAwait(false);

            await _userSubscriptionService.AddAsync(userSubscription).ConfigureAwait(false);

            Logger.Info($"Purchase billing for product '{productId}' was registered.");
        }
    }
}
