﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.InAppBilling;
using Prism.Navigation;
using RewriteMe.Business.Extensions;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Exceptions;
using RewriteMe.Domain.Interfaces.Configuration;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Logging.Extensions;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Transcription;
using RewriteMe.Resources.Localization;
using Xamarin.Forms;

namespace RewriteMe.Mobile.ViewModels
{
    public class UserSubscriptionsPageViewModel : ViewModelBase
    {
        private const string PurchaseSubscription = "Purchase subscription";
        private const string PendingPurchaseSubscription = "Pending purchase subscription";
        private const string StartPurchasingSubscription = "Start purchasing subscription";

        private readonly IUserSubscriptionService _userSubscriptionService;
        private readonly IBillingPurchaseService _billingPurchaseService;
        private readonly IConnectivityService _connectivityService;
        private readonly IAppCenterMetricsService _appCenterMetricsService;
        private readonly IEmailService _emailService;
        private readonly IDeviceService _deviceService;
        private readonly IApplicationSettings _applicationSettings;
        private readonly IApplicationVersionProvider _applicationVersionProvider;
        private readonly IInAppBilling _inAppBilling;

        private IList<SubscriptionProductViewModel> _products;
        private string _remainingTime;

        public UserSubscriptionsPageViewModel(
            IUserSubscriptionService userSubscriptionService,
            IBillingPurchaseService billingPurchaseService,
            IConnectivityService connectivityService,
            IAppCenterMetricsService appCenterMetricsService,
            IEmailService emailService,
            IDeviceService deviceService,
            IApplicationSettings applicationSettings,
            IApplicationVersionProvider applicationVersionProvider,
            IInAppBilling inAppBilling,
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
            _deviceService = deviceService;
            _emailService = emailService;
            _applicationSettings = applicationSettings;
            _applicationVersionProvider = applicationVersionProvider;
            _inAppBilling = inAppBilling;

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
                    if (!CrossInAppBilling.IsSupported)
                    {
                        await DialogService.AlertAsync(Loc.Text(TranslationKeys.InAppBillingIsNotSupportedErrorMessage)).ConfigureAwait(false);
                    }

                    await CheckPendingPurchasesAsync().ConfigureAwait(false);
                    await InitializeRemainingTimeAsync().ConfigureAwait(false);
                    await InitializeProductsAsync().ConfigureAwait(false);
                }
            }
        }

        private async Task CheckPendingPurchasesAsync()
        {
            try
            {
                var result = await _billingPurchaseService.HandlePendingPurchases().ConfigureAwait(false);
                if (result.HasValue && result.Value)
                {
                    await DialogService.AlertAsync(Loc.Text(TranslationKeys.SubscriptionWasSuccessfullyPurchased)).ConfigureAwait(false);
                    await InitializeRemainingTimeAsync().ConfigureAwait(false);
                }
                else if (!result.HasValue)
                {
                    var pendingPurchases = await _billingPurchaseService.GetAllPaymentPendingAsync().ConfigureAwait(false);
                    if (!pendingPurchases.Any())
                    {
                        await DialogService.AlertAsync(Loc.Text(TranslationKeys.PurchaseNotFoundErrorMessage)).ConfigureAwait(false);
                    }
                }
            }
            catch (AppStoreNotConnectedException ex)
            {
                TrackException(ex);
                Logger.Error("App store is not connected.");
                Logger.Error(ExceptionFormatter.FormatException(ex));

                await DialogService.AlertAsync(Loc.Text(TranslationKeys.AppStoreUnavailableErrorMessage)).ConfigureAwait(false);
            }
            catch (NoPurchasesInStoreException ex)
            {
                TrackException(ex);
                Logger.Error("Purchases not found in the store.");
                Logger.Error(ExceptionFormatter.FormatException(ex));

                await DialogService.AlertAsync(Loc.Text(TranslationKeys.PurchaseNotFoundErrorMessage)).ConfigureAwait(false);
            }
            catch (PurchaseNotFoundException ex)
            {
                TrackException(ex);
                Logger.Error($"Purchase {ex.PurchaseId} not found in the store.");
                Logger.Error(ExceptionFormatter.FormatException(ex));

                var result = await DialogService.ConfirmAsync(
                    Loc.Text(TranslationKeys.PurchaseNotFoundErrorMessage),
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
            catch (PurchaseWasNotProcessedException ex)
            {
                TrackException(ex);
                Logger.Error("Product was not purchased.");
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
            catch (Exception ex)
            {
                TrackException(ex);
                Logger.Error("Handling pending payments failed.");
                Logger.Error(ExceptionFormatter.FormatException(ex));
            }

            var anyPendingPurchases = await _billingPurchaseService.GetAllPaymentPendingAsync().ConfigureAwait(false);
            if (anyPendingPurchases.Any())
            {
                await DialogService.AlertAsync(Loc.Text(TranslationKeys.PendingPaymentErrorMessage)).ConfigureAwait(false);
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
                    return;

                var connected = await _inAppBilling.ConnectAsync().ConfigureAwait(false);
                if (!connected)
                {
                    await DialogService.AlertAsync(Loc.Text(TranslationKeys.AppStoreUnavailableErrorMessage)).ConfigureAwait(false);
                    return;
                }

                var inAppBillingProducts = await _inAppBilling.GetProductInfoAsync(ItemType.InAppPurchase, SubscriptionProducts.All.Select(x => x.ProductId).ToArray()).ConfigureAwait(false);
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
                await _inAppBilling.DisconnectAsync().ConfigureAwait(false);
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

                var connected = await _inAppBilling.ConnectAsync().ConfigureAwait(false);
                if (!connected)
                    throw new AppStoreNotConnectedException();

                var purchase = await _inAppBilling
                    .PurchaseAsync(productId, ItemType.InAppPurchase)
                    .ConfigureAwait(false);

                using (new OperationMonitor(OperationScope))
                {
                    if (purchase == null)
                        throw new PurchaseWasNotProcessedException();

                    var orderId = purchase.Id;
                    if (purchase.State == PurchaseState.PaymentPending)
                    {
                        Logger.Info("Product was purchased but payment is pending.");

                        await TrackEvent(PendingPurchaseSubscription, productId).ConfigureAwait(false);
                        await _billingPurchaseService.AddAsync(purchase).ConfigureAwait(false);
                        await DialogService.AlertAsync(Loc.Text(TranslationKeys.PendingPaymentErrorMessage)).ConfigureAwait(false);

                        try
                        {
                            await SendBillingPurchaseAsync(orderId, productId, purchase).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error("Exception during registration of purchase billing.");
                            Logger.Error(ExceptionFormatter.FormatException(ex));
                        }

                        return false;
                    }
                    else if (purchase.State == PurchaseState.Purchased)
                    {
                        if (string.IsNullOrWhiteSpace(purchase.PurchaseToken))
                            throw new EmptyPurchaseTokenException(purchase.Id, purchase.ProductId);

                        var isConsumed = await _inAppBilling.ConsumePurchaseAsync(purchase.ProductId, purchase.PurchaseToken).ConfigureAwait(false);
                        if (!isConsumed)
                        {
                            Logger.Info($"Product '{productId}' was purchased.");
                        }
                        else
                        {
                            purchase.ConsumptionState = ConsumptionState.Consumed;
                        }

                        await SendBillingPurchaseAsync(orderId, productId, purchase).ConfigureAwait(false);
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
                await _inAppBilling.DisconnectAsync().ConfigureAwait(false);
            }

            return false;
        }

        private async Task SendBillingPurchaseAsync(string orderId, string productId, InAppBillingPurchase purchase)
        {
            try
            {
                var userId = await UserSessionService.GetUserIdAsync().ConfigureAwait(false);
                var billingPurchase = purchase.ToUserSubscriptionModel(userId, orderId, _deviceService.RuntimePlatform);
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
