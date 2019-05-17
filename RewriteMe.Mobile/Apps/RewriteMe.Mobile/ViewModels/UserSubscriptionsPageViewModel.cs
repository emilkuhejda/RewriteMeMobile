using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.InAppBilling;
using Plugin.InAppBilling.Abstractions;
using Plugin.LatestVersion.Abstractions;
using Plugin.Messaging;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Exceptions;
using RewriteMe.Domain.Interfaces.Configuration;
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
        private readonly IApplicationSettings _applicationSettings;
        private readonly ILatestVersion _latestVersion;
        private readonly IEmailTask _emailTask;

        private IList<SubscriptionProductViewModel> _products;

        public UserSubscriptionsPageViewModel(
            IUserSessionService userSessionService,
            IUserSubscriptionService userSubscriptionService,
            IBillingPurchaseService billingPurchaseService,
            IApplicationSettings applicationSettings,
            ILatestVersion latestVersion,
            IEmailTask emailTask,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(dialogService, navigationService, loggerFactory)
        {
            _userSessionService = userSessionService;
            _userSubscriptionService = userSubscriptionService;
            _billingPurchaseService = billingPurchaseService;
            _applicationSettings = applicationSettings;
            _latestVersion = latestVersion;
            _emailTask = emailTask;

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
            var result = await MakePurchaseAsync(productId).ConfigureAwait(false);
            if (!result)
                return;

            Logger.Info("Subscription was successfully purchased and registered.");
            await DialogService.AlertAsync(Loc.Text(TranslationKeys.SubscriptionWasSuccessfullyPurchased)).ConfigureAwait(false);
        }

        public async Task<bool> MakePurchaseAsync(string productId)
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
                        throw new PurchasePayloadNotValidException(purchase);

                    InAppBillingPurchase billingPurchase = null;
                    if (Device.RuntimePlatform == Device.iOS)
                    {
                        Logger.Info($"Product '{productId}' was purchased.");

                        billingPurchase = purchase;
                    }
                    else
                    {
                        var consumedItem = await billing.ConsumePurchaseAsync(purchase.ProductId, purchase.PurchaseToken).ConfigureAwait(false);
                        if (consumedItem != null)
                        {
                            if (consumedItem.Payload == payload.ToString())
                                throw new PurchasePayloadNotValidException(purchase);

                            Logger.Info($"Product '{productId}' was purchased.");

                            billingPurchase = consumedItem;
                        }
                    }

                    if (billingPurchase == null)
                        throw new PurchaseWasNotProcessedException();

                    await SendBillingPurchaseAsync(productId, billingPurchase).ConfigureAwait(false);
                }

                throw new PurchaseWasNotProcessedException();
            }
            catch (PurchaseWasNotProcessedException ex)
            {
                Logger.Warning($"Product '{productId}' was not purchased. {ex}");

                await DialogService.AlertAsync(Loc.Text(TranslationKeys.PurchaseWasNotProcessedErrorMessage)).ConfigureAwait(false);
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
                    await CreateContactUsMailAsync(ex.BillingPurchase).ConfigureAwait(false);
                }
            }
            catch (RegistrationPurchaseBillingException ex)
            {
                Logger.Error($"Exception during registration of purchase billing. {ex}");

                var result = await DialogService.ConfirmAsync(
                    Loc.Text(TranslationKeys.RegistrationPurchaseBillingErrorMessage),
                    okText: Loc.Text(TranslationKeys.SendEmail),
                    cancelText: Loc.Text(TranslationKeys.Ok)).ConfigureAwait(false);

                if (result)
                {
                    await CreateContactUsMailAsync(ex.BillingPurchase).ConfigureAwait(false);
                }
            }
            catch (InAppBillingPurchaseException ex)
            {
                if (ex.PurchaseError == PurchaseError.UserCancelled)
                    return false;

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

            return false;
        }

        private async Task SendBillingPurchaseAsync(string productId, InAppBillingPurchase purchase)
        {
            try
            {
                var userId = await _userSessionService.GetUserIdAsync().ConfigureAwait(false);
                var userSubscription = await _billingPurchaseService
                    .SendBillingPurchaseAsync(purchase.ToBillingPurchase(userId)).ConfigureAwait(false);

                await _userSubscriptionService.AddAsync(userSubscription).ConfigureAwait(false);

                Logger.Info($"Purchase billing for product '{productId}' was registered.");
            }
            catch (ErrorRequestException ex)
            {
                throw new RegistrationPurchaseBillingException(purchase, nameof(purchase), ex);
            }
        }

        private async Task CreateContactUsMailAsync(InAppBillingPurchase purchase)
        {
            if (purchase == null)
                return;

            if (string.IsNullOrWhiteSpace(_applicationSettings.SupportMailAddress))
                return;

            if (_emailTask.CanSendEmail)
            {
                var userId = await _userSessionService.GetUserIdAsync().ConfigureAwait(false);
                var subject = $"{Loc.Text(TranslationKeys.ApplicationTitle)} - Purchase problem";
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss \"GMT\"zzz", CultureInfo.InvariantCulture);
                var message = new StringBuilder()
                    .AppendLine()
                    .AppendLine()
                    .AppendLine()
                    .AppendLine()
                    .AppendLine()
                    .AppendLine()
                    .AppendLine()
                    .AppendLine("_______________________________________")
                    .AppendLine($"Order Id: {purchase.Id}")
                    .AppendLine($"User subscription: {purchase.ProductId}")
                    .AppendLine($"User identification: {userId}")
                    .AppendLine($"Application version: {_latestVersion.InstalledVersionNumber} ({Device.RuntimePlatform})")
                    .AppendLine($"Time stamp: {timestamp}")
                    .ToString();

                _emailTask.SendEmail(_applicationSettings.SupportMailAddress, subject, message);
            }
            else
            {
                await DialogService.AlertAsync(Loc.Text(TranslationKeys.EmailIsNotSupported)).ConfigureAwait(false);
            }
        }
    }
}
