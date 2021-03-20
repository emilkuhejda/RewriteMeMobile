using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.InAppBilling;
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
        private const string PurchaseSubscription = "Purchase subscription";
        private const string StartPurchasingSubscription = "Start purchasing subscription";

        private readonly IUserSubscriptionService _userSubscriptionService;
        private readonly IBillingPurchaseService _billingPurchaseService;
        private readonly IConnectivityService _connectivityService;
        private readonly IAppCenterMetricsService _appCenterMetricsService;
        private readonly IEmailService _emailService;
        private readonly IApplicationSettings _applicationSettings;
        private readonly IApplicationVersionProvider _applicationVersionProvider;

        private IList<SubscriptionProductViewModel> _products;
        private string _remainingTime;

        public UserSubscriptionsPageViewModel(
            IUserSubscriptionService userSubscriptionService,
            IBillingPurchaseService billingPurchaseService,
            IConnectivityService connectivityService,
            IAppCenterMetricsService appCenterMetricsService,
            IEmailService emailService,
            IApplicationSettings applicationSettings,
            IApplicationVersionProvider applicationVersionProvider,
            IUserSessionService userSessionService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(userSessionService, dialogService, navigationService, loggerFactory)
        {
            _userSubscriptionService = userSubscriptionService;
            _billingPurchaseService = billingPurchaseService;
            _connectivityService = connectivityService;
            _appCenterMetricsService = appCenterMetricsService;
            _emailService = emailService;
            _applicationSettings = applicationSettings;
            _applicationVersionProvider = applicationVersionProvider;

            CanGoBack = true;
        }

        public IList<SubscriptionProductViewModel> Products
        {
            get => _products;
            private set => SetProperty(ref _products, value);
        }

        public string RemainingTime
        {
            get => _remainingTime;
            set => SetProperty(ref _remainingTime, value);
        }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                if (navigationParameters.GetNavigationMode() == NavigationMode.New)
                {
                    await InitializeRemainingTimeAsync().ConfigureAwait(false);
                    await InitializeProductsAsync().ConfigureAwait(false);
                }
            }
        }

        private async Task InitializeRemainingTimeAsync()
        {
            var remainingTime = await _userSubscriptionService.GetRemainingTimeAsync().ConfigureAwait(false);
            var sign = remainingTime.Ticks < 0 ? "-" : string.Empty;
            RemainingTime = $"{sign}{remainingTime:hh\\:mm\\:ss}";
        }

        private async Task InitializeProductsAsync()
        {
            if (!_connectivityService.IsConnected)
                return;

            try
            {
                if (!CrossInAppBilling.IsSupported)
                {
                    await DialogService.AlertAsync(Loc.Text(TranslationKeys.InAppBillingIsNotSupportedErrorMessage)).ConfigureAwait(false);
                    return;
                }

                var billing = CrossInAppBilling.Current;
                var connected = await billing.ConnectAsync().ConfigureAwait(false);
                if (!connected)
                {
                    await DialogService.AlertAsync(Loc.Text(TranslationKeys.AppStoreUnavailableErrorMessage)).ConfigureAwait(false);
                    return;
                }

                var inAppBillingProducts = await billing.GetProductInfoAsync(ItemType.InAppPurchase, SubscriptionProducts.All.Select(x => x.ProductId).ToArray()).ConfigureAwait(false);
                var billingProducts = inAppBillingProducts.ToList();

                var products = new List<SubscriptionProductViewModel>();
                foreach (var subscription in SubscriptionProducts.All)
                {
                    var appBillingProduct = billingProducts.FirstOrDefault(x => x.ProductId == subscription.ProductId);
                    if (appBillingProduct == null)
                        continue;

                    var subscriptionProductViewModel = new SubscriptionProductViewModel(appBillingProduct.ProductId, OnBuyAction)
                    {
                        Title = subscription.Text,
                        Price = appBillingProduct.LocalizedPrice,
                        Description = subscription.Description
                    };

                    products.Add(subscriptionProductViewModel);
                }

                Products = products;
            }
            catch (Exception ex)
            {
                TrackException(ex);
                Logger.Error("Connection to App Store failed.");
                Logger.Error(ExceptionFormatter.FormatException(ex));

                await DialogService.AlertAsync(Loc.Text(TranslationKeys.AppStoreUnavailableErrorMessage)).ConfigureAwait(false);
            }
            finally
            {
                await CrossInAppBilling.Current.DisconnectAsync().ConfigureAwait(false);
            }
        }

        public async Task OnBuyAction(string productId)
        {
            var result = await MakePurchaseAsync(productId).ConfigureAwait(false);
            if (!result)
                return;

            Logger.Info("Subscription was successfully purchased and registered.");
            await TrackEvent(PurchaseSubscription, productId).ConfigureAwait(false);
            await DialogService.AlertAsync(Loc.Text(TranslationKeys.SubscriptionWasSuccessfullyPurchased)).ConfigureAwait(false);
            await InitializeRemainingTimeAsync().ConfigureAwait(false);
        }

        public async Task<bool> MakePurchaseAsync(string productId)
        {
            try
            {
                if (!CrossInAppBilling.IsSupported)
                    throw new InAppBillingNotSupportedException();

                await TrackEvent(StartPurchasingSubscription, productId).ConfigureAwait(false);
                Logger.Info($"Start purchasing product '{productId}'.");

                var payload = Guid.NewGuid().ToString();
                var billing = CrossInAppBilling.Current;
                var connected = await billing.ConnectAsync().ConfigureAwait(false);
                if (!connected)
                    throw new AppStoreNotConnectedException();

                var purchase = await billing
                    .PurchaseAsync(productId, ItemType.InAppPurchase, payload)
                    .ConfigureAwait(false);

                using (new OperationMonitor(OperationScope))
                {
                    if (purchase != null && purchase.State == PurchaseState.Purchased)
                    {
                        if (string.IsNullOrWhiteSpace(purchase.PurchaseToken))
                            throw new EmptyPurchaseTokenException(purchase.Id, purchase.ProductId);

                        var orderId = purchase.Id;
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
                                Logger.Info($"Product '{productId}' was purchased.");

                                billingPurchase = consumedItem;
                            }
                        }

                        if (billingPurchase == null)
                            throw new PurchaseWasNotProcessedException();

                        await SendBillingPurchaseAsync(orderId, productId, billingPurchase).ConfigureAwait(false);

                        return true;
                    }

                    throw new PurchaseWasNotProcessedException();
                }
            }
            catch (PurchaseWasNotProcessedException ex)
            {
                TrackException(ex);
                Logger.Error("Product '{productId}' was not purchased.");
                Logger.Error(ExceptionFormatter.FormatException(ex));

                await DialogService.AlertAsync(Loc.Text(TranslationKeys.PurchaseWasNotProcessedErrorMessage)).ConfigureAwait(false);
            }
            catch (InAppBillingNotSupportedException ex)
            {
                TrackException(ex);
                Logger.Error("In-App Purchases is not supported in the device.");
                Logger.Error(ExceptionFormatter.FormatException(ex));

                await DialogService.AlertAsync(Loc.Text(TranslationKeys.InAppBillingIsNotSupportedErrorMessage)).ConfigureAwait(false);
            }
            catch (AppStoreNotConnectedException ex)
            {
                TrackException(ex);
                Logger.Error("App store is not connected.");
                Logger.Error(ExceptionFormatter.FormatException(ex));

                await DialogService.AlertAsync(Loc.Text(TranslationKeys.AppStoreUnavailableErrorMessage)).ConfigureAwait(false);
            }
            catch (EmptyPurchaseTokenException ex)
            {
                TrackException(ex);
                Logger.Error("Purchase token is empty.");
                Logger.Error(ExceptionFormatter.FormatException(ex));

                var result = await DialogService.ConfirmAsync(
                    Loc.Text(TranslationKeys.PurchaseProcessedIncorrectlyErrorMessage),
                    okText: Loc.Text(TranslationKeys.SendEmail),
                    cancelText: Loc.Text(TranslationKeys.Ok)).ConfigureAwait(false);

                if (result)
                {
                    await CreateContactUsMailAsync(ex.PurchaseId, ex.ProductId).ConfigureAwait(false);
                }
            }
            catch (RegistrationPurchaseBillingException ex)
            {
                TrackException(ex);
                Logger.Error("Exception during registration of purchase billing.");
                Logger.Error(ExceptionFormatter.FormatException(ex));

                var result = await DialogService.ConfirmAsync(
                    Loc.Text(TranslationKeys.RegistrationPurchaseBillingErrorMessage),
                    okText: Loc.Text(TranslationKeys.SendEmail),
                    cancelText: Loc.Text(TranslationKeys.Ok)).ConfigureAwait(false);

                if (result)
                {
                    await CreateContactUsMailAsync(ex.PurchaseId, ex.ProductId).ConfigureAwait(false);
                }
            }
            catch (InAppBillingPurchaseException ex)
            {
                if (ex.PurchaseError == PurchaseError.UserCancelled)
                    return false;

                TrackException(ex);
                Logger.Error("Exception during purchasing process.");
                Logger.Error(ExceptionFormatter.FormatException(ex));

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
                TrackException(ex);
                Logger.Error("Exception during purchasing process.");
                Logger.Error(ExceptionFormatter.FormatException(ex));

                await DialogService.AlertAsync(Loc.Text(TranslationKeys.PurchaseWasNotProcessedErrorMessage)).ConfigureAwait(false);
            }
            finally
            {
                await CrossInAppBilling.Current.DisconnectAsync().ConfigureAwait(false);
            }

            return false;
        }

        private async Task SendBillingPurchaseAsync(string orderId, string productId, InAppBillingPurchase purchase)
        {
            try
            {
                var userId = await UserSessionService.GetUserIdAsync().ConfigureAwait(false);
                var billingPurchase = purchase.ToUserSubscriptionModel(userId, orderId);
                var remainingTime = await _billingPurchaseService.SendBillingPurchaseAsync(billingPurchase).ConfigureAwait(false);

                await _userSubscriptionService.UpdateRemainingTimeAsync(remainingTime.Time).ConfigureAwait(false);

                Logger.Info($"Purchase billing for product '{productId}' was registered.");
            }
            catch (OfflineRequestException ex)
            {
                throw new RegistrationPurchaseBillingException(purchase.Id, purchase.ProductId, nameof(purchase), ex);
            }
            catch (ErrorRequestException ex)
            {
                throw new RegistrationPurchaseBillingException(purchase.Id, purchase.ProductId, nameof(purchase), ex);
            }
        }

        private async Task CreateContactUsMailAsync(string purchaseId, string productId)
        {
            if (string.IsNullOrWhiteSpace(_applicationSettings.SupportMailAddress))
                return;

            var userId = await UserSessionService.GetUserIdAsync().ConfigureAwait(false);
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
                .AppendLine($"Order Id: {purchaseId}")
                .AppendLine($"User subscription: {productId}")
                .AppendLine($"User identification: {userId}")
                .AppendLine($"Application version: {_applicationVersionProvider.GetInstalledVersionNumber()} ({Device.RuntimePlatform})")
                .AppendLine($"Time stamp: {timestamp}")
                .ToString();

            await _emailService.SendAsync(_applicationSettings.SupportMailAddress, subject, message).ConfigureAwait(false);
        }

        private async Task TrackEvent(string name, string productId)
        {
            var userId = await UserSessionService.GetUserIdAsync().ConfigureAwait(false);
            var properties = new Dictionary<string, string> {
                { "User ID", userId.ToString() },
                { "Product ID", productId }
            };

            _appCenterMetricsService.TrackEvent(name, properties);
        }

        private void TrackException(Exception ex)
        {
            _appCenterMetricsService.TrackException(ex);
        }
    }
}
