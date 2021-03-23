using System;
using System.Threading.Tasks;
using Moq;
using Plugin.InAppBilling;
using RewriteMe.Business.Services;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.Interfaces.Repositories;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.WebApi;
using Xunit;

namespace RewriteMe.Business.Tests.Services
{
    public class BillingPurchaseServiceTests
    {
        [Fact]
        public async Task HandlePendingPurchases_ProcessPendingPurchases()
        {
            // Arrange
            var userSessionServiceMock = new Mock<IUserSessionService>();
            var userSubscriptionServiceMock = new Mock<IUserSubscriptionService>();
            var connectivityServiceMock = new Mock<IConnectivityService>();
            var deviceServiceMock = new Mock<IDeviceService>();
            var rewriteMeWebServiceMock = new Mock<IRewriteMeWebService>();
            var inAppBillingMock = new Mock<IInAppBilling>();
            var billingPurchaseRepositoryMock = new Mock<IBillingPurchaseRepository>();

            var billingPurchase = CreateInAppBillingPurchase(PurchaseState.Pending);
            var billingPurchaseInStore = CreateInAppBillingPurchase(PurchaseState.Purchased);
            var expectedRemainingTime = TimeSpan.FromMinutes(1);

            connectivityServiceMock.Setup(x => x.IsConnected).Returns(true);
            deviceServiceMock
                .Setup(x => x.RuntimePlatform)
                .Returns("Android");
            rewriteMeWebServiceMock
                .Setup(x => x.CreateUserSubscriptionAsync(It.IsAny<CreateUserSubscriptionInputModel>()))
                .ReturnsAsync(new HttpRequestResult<TimeSpanWrapper>(HttpRequestState.Success, 200, new TimeSpanWrapper { Ticks = expectedRemainingTime.Ticks }));
            inAppBillingMock
                .Setup(x => x.ConnectAsync(It.IsAny<bool>()))
                .ReturnsAsync(true);
            inAppBillingMock
                .Setup(x => x.ConsumePurchaseAsync(It.Is<string>(p => p == billingPurchase.ProductId), It.Is<string>(t => t == billingPurchase.PurchaseToken)))
                .ReturnsAsync(true);
            inAppBillingMock
                .Setup(x => x.GetPurchasesAsync(It.IsAny<ItemType>()))
                .ReturnsAsync(new[] { billingPurchaseInStore });
            billingPurchaseRepositoryMock
                .Setup(x => x.GetAllPaymentPendingAsync())
                .ReturnsAsync(new[] { billingPurchase });

            var billingPurchaseService = new BillingPurchaseService(
                userSessionServiceMock.Object,
                userSubscriptionServiceMock.Object,
                connectivityServiceMock.Object,
                deviceServiceMock.Object,
                rewriteMeWebServiceMock.Object,
                inAppBillingMock.Object,
                billingPurchaseRepositoryMock.Object);

            // Act
            await billingPurchaseService.HandlePendingPurchases();

            // Assert
            userSubscriptionServiceMock.Verify(x => x.UpdateRemainingTimeAsync(It.Is<TimeSpan>(ts => ts == expectedRemainingTime)));
            billingPurchaseRepositoryMock.Verify(x => x.UpdateAsync(It.Is<InAppBillingPurchase>(p => p.Id == billingPurchase.Id)));
        }

        private InAppBillingPurchase CreateInAppBillingPurchase(PurchaseState purchaseState)
        {
            return new InAppBillingPurchase
            {
                Id = "GPA.3333-8958-6620-09100",
                ProductId = "product.subscription.v1.basic",
                AutoRenewing = false,
                PurchaseToken = string.Empty,
                Payload = "payload",
                State = purchaseState,
                ConsumptionState = ConsumptionState.Consumed,
                IsAcknowledged = false,
                TransactionDateUtc = DateTime.UtcNow
            };
        }
    }
}
