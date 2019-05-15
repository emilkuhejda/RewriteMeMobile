using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.InAppBilling;
using Plugin.InAppBilling.Abstractions;
using RewriteMe.Domain.Exceptions;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Transcription;
using RewriteMe.Logging.Extensions;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Resources.Localization;
using Xamarin.Forms;

namespace RewriteMe.Mobile.ViewModels
{
    public class SubscriptionProductViewModel
    {
        private readonly IUserSessionService _userSessionService;
        private readonly IUserSubscriptionService _userSubscriptionService;
        private readonly IBillingPurchaseService _billingPurchaseService;
        private readonly IDialogService _dialogService;
        private readonly ILogger _logger;

        public SubscriptionProductViewModel(
            SubscriptionProduct subscriptionProduct,
            IUserSessionService userSessionService,
            IUserSubscriptionService userSubscriptionService,
            IBillingPurchaseService billingPurchaseService,
            IDialogService dialogService,
            ILoggerFactory loggerFactory)
        {
            _userSessionService = userSessionService;
            _userSubscriptionService = userSubscriptionService;
            _billingPurchaseService = billingPurchaseService;
            _dialogService = dialogService;
            _logger = loggerFactory.CreateLogger(typeof(SubscriptionProductViewModel));

            SubscriptionProduct = subscriptionProduct;

            BuyCommand = new AsyncCommand(ExecuteBuyCommandAsync);
        }

        private SubscriptionProduct SubscriptionProduct { get; }

        public string Text => SubscriptionProduct.Text;

        public ICommand BuyCommand { get; }

        private async Task ExecuteBuyCommandAsync()
        {
            try
            {
                if (!CrossInAppBilling.IsSupported)
                    throw new InAppBillingNotSupportedException();

                _logger.Info($"Start purchasing product '{SubscriptionProduct.Id}'.");

                var payload = Guid.NewGuid();
                var billing = CrossInAppBilling.Current;
                var connected = await billing.ConnectAsync().ConfigureAwait(false);
                if (!connected)
                    throw new AppStoreNotConnectedException();

                var purchase = await billing
                    .PurchaseAsync(SubscriptionProduct.Id, ItemType.InAppPurchase, payload.ToString())
                    .ConfigureAwait(false);

                if (purchase != null && purchase.State == PurchaseState.Purchased)
                {
                    if (purchase.Payload == payload.ToString())
                        throw new PurchasePayloadNotValidException(nameof(payload));

                    InAppBillingPurchase billingPurchase = null;
                    if (Device.RuntimePlatform == Device.iOS)
                    {
                        _logger.Info($"Product '{SubscriptionProduct.Id}' was purchased.");

                        billingPurchase = purchase;
                    }
                    else
                    {
                        var consumedItem = await billing
                            .ConsumePurchaseAsync(purchase.ProductId, purchase.PurchaseToken).ConfigureAwait(false);
                        if (consumedItem != null)
                        {
                            _logger.Info($"Product '{SubscriptionProduct.Id}' was purchased.");

                            billingPurchase = consumedItem;
                        }
                    }

                    if (billingPurchase != null)
                    {
                        await SendBillingPurchaseAsync(SubscriptionProduct.Id, billingPurchase).ConfigureAwait(false);
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
                _logger.Warning($"In-App Purchases is not supported in the device. {ex}");
                await _dialogService.AlertAsync(Loc.Text(TranslationKeys.InAppBillingIsNotSupportedErrorMessage)).ConfigureAwait(false);
            }
            catch (AppStoreNotConnectedException ex)
            {
                _logger.Error($"App store is not connected. {ex}");
                await _dialogService.AlertAsync(Loc.Text(TranslationKeys.AppStoreUnavailableErrorMessage)).ConfigureAwait(false);
            }
            catch (PurchasePayloadNotValidException ex)
            {
                _logger.Error($"Purchase payload is not valid. {ex}");

                var result = await _dialogService.ConfirmAsync(
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
                _logger.Warning($"Product '{SubscriptionProduct.Id}' was not purchased. {ex}");

                await _dialogService.AlertAsync(Loc.Text(TranslationKeys.PurchaseWasNotProcessedErrorMessage)).ConfigureAwait(false);
            }
            catch (ErrorRequestException ex)
            {
                _logger.Error($"Exception during registration of purchase billing. {ex}");

                var result = await _dialogService.ConfirmAsync(
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
                _logger.Error($"Exception during purchasing process. {ex}");

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

                await _dialogService.AlertAsync(message).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception during purchasing process. {ex}");

                await _dialogService.AlertAsync(Loc.Text(TranslationKeys.PurchaseWasNotProcessedErrorMessage)).ConfigureAwait(false);
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

            _logger.Info($"Purchase billing for product '{productId}' was registered.");
        }
    }
}
