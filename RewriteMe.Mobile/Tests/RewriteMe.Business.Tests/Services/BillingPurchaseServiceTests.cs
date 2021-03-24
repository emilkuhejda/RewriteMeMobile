using System;
using System.Threading.Tasks;
using Moq;
using Plugin.InAppBilling;
using RewriteMe.Business.Services;
using RewriteMe.Domain.Exceptions;
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
            billingPurchase.TransactionDateUtc = billingPurchase.TransactionDateUtc.AddMinutes(-6);
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
            var result = await billingPurchaseService.HandlePendingPurchases();

            // Assert
            Assert.True(result);
            userSubscriptionServiceMock
                .Verify(x => x.UpdateRemainingTimeAsync(It.Is<TimeSpan>(ts => ts == expectedRemainingTime)), Times.Once);
            billingPurchaseRepositoryMock
                .Verify(
                    x => x.UpdateAsync(It.Is<InAppBillingPurchase>(p =>
                        p.Id == billingPurchase.Id && p.State == PurchaseState.Purchased)), Times.Once);
        }

        [Fact]
        public async Task HandlePendingPurchases_ConsumePurchaseThrowsException_PurchaseIsStillPending()
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
            billingPurchase.TransactionDateUtc = billingPurchase.TransactionDateUtc.AddMinutes(-6);
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
                .Throws(new Exception());
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
            var result = await billingPurchaseService.HandlePendingPurchases();

            // Assert
            Assert.Null(result);
            userSubscriptionServiceMock
                .Verify(x => x.UpdateRemainingTimeAsync(It.Is<TimeSpan>(ts => ts == expectedRemainingTime)), Times.Never);
            billingPurchaseRepositoryMock
                .Verify(x => x.UpdateAsync(It.Is<InAppBillingPurchase>(p => p.Id == billingPurchase.Id)), Times.Never);
        }

        [Fact]
        public async Task HandlePendingPurchases_ConsumePurchaseReturnsFalse_PurchaseIsStillPending()
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
            billingPurchase.TransactionDateUtc = billingPurchase.TransactionDateUtc.AddMinutes(-6);
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
                .ReturnsAsync(false);
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
            var result = await billingPurchaseService.HandlePendingPurchases();

            // Assert
            Assert.False(result);
            userSubscriptionServiceMock
                .Verify(x => x.UpdateRemainingTimeAsync(It.Is<TimeSpan>(ts => ts == expectedRemainingTime)), Times.Never);
            billingPurchaseRepositoryMock
                .Verify(x => x.UpdateAsync(It.Is<InAppBillingPurchase>(p => p.Id == billingPurchase.Id)), Times.Never);
        }

        [Fact]
        public async Task HandlePendingPurchases_PurchaseTransactionDateLessThanFiveMinutes_NoAction()
        {
            // Arrange
            var userSessionServiceMock = new Mock<IUserSessionService>();
            var userSubscriptionServiceMock = new Mock<IUserSubscriptionService>();
            var connectivityServiceMock = new Mock<IConnectivityService>();
            var deviceServiceMock = new Mock<IDeviceService>();
            var rewriteMeWebServiceMock = new Mock<IRewriteMeWebService>();
            var inAppBillingMock = new Mock<IInAppBilling>();
            var billingPurchaseRepositoryMock = new Mock<IBillingPurchaseRepository>();

            var billingPurchaseInStore = CreateInAppBillingPurchase(PurchaseState.Purchased);
            var billingPurchase = CreateInAppBillingPurchase(PurchaseState.Pending);
            billingPurchase.TransactionDateUtc = billingPurchaseInStore.TransactionDateUtc.AddMinutes(-4.5);

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
            userSubscriptionServiceMock
                .Verify(x => x.UpdateRemainingTimeAsync(It.Is<TimeSpan>(ts => ts == expectedRemainingTime)), Times.Never);
            billingPurchaseRepositoryMock
                .Verify(x => x.UpdateAsync(It.Is<InAppBillingPurchase>(p => p.Id == billingPurchase.Id)), Times.Never);
        }

        [Fact]
        public async Task HandlePendingPurchases_NoPurchaseInTheStore_ConsumesPurchase()
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
            billingPurchase.TransactionDateUtc = billingPurchase.TransactionDateUtc.AddMinutes(-6);
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
                .ReturnsAsync(Array.Empty<InAppBillingPurchase>());
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
            userSubscriptionServiceMock
                .Verify(x => x.UpdateRemainingTimeAsync(It.Is<TimeSpan>(ts => ts == expectedRemainingTime)), Times.Once);
            billingPurchaseRepositoryMock
                .Verify(
                    x => x.UpdateAsync(It.Is<InAppBillingPurchase>(p =>
                        p.Id == billingPurchase.Id && p.State == PurchaseState.Purchased)), Times.Once);
        }

        [Fact]
        public async Task HandlePendingPurchases_NoPurchaseInTheStore_ConsumePurchaseThrowsException_ThrowsNoPurchasesException()
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
            billingPurchase.TransactionDateUtc = billingPurchase.TransactionDateUtc.AddMinutes(-6);
            var expectedRemainingTime = TimeSpan.FromMinutes(0);

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
                .Throws(new Exception());
            inAppBillingMock
                .Setup(x => x.GetPurchasesAsync(It.IsAny<ItemType>()))
                .ReturnsAsync(Array.Empty<InAppBillingPurchase>());
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

            // Act & Assert
            await Assert.ThrowsAsync<NoPurchasesInStoreException>(async () => await billingPurchaseService.HandlePendingPurchases());

            // Assert
            userSubscriptionServiceMock
                .Verify(x => x.UpdateRemainingTimeAsync(It.Is<TimeSpan>(ts => ts == expectedRemainingTime)), Times.Once);
            billingPurchaseRepositoryMock
                .Verify(
                    x => x.UpdateAsync(It.Is<InAppBillingPurchase>(p =>
                        p.Id == billingPurchase.Id && p.State == PurchaseState.Failed)), Times.Once);
        }

        [Fact]
        public async Task HandlePendingPurchases_NoPurchaseInTheStore_ConsumePurchaseReturnsFalse_ThrowsNoPurchasesException()
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
            billingPurchase.TransactionDateUtc = billingPurchase.TransactionDateUtc.AddMinutes(-6);
            var expectedRemainingTime = TimeSpan.FromMinutes(0);

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
                .ReturnsAsync(false);
            inAppBillingMock
                .Setup(x => x.GetPurchasesAsync(It.IsAny<ItemType>()))
                .ReturnsAsync(Array.Empty<InAppBillingPurchase>());
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

            // Act & Assert
            await Assert.ThrowsAsync<NoPurchasesInStoreException>(async () => await billingPurchaseService.HandlePendingPurchases());

            // Assert
            userSubscriptionServiceMock
                .Verify(x => x.UpdateRemainingTimeAsync(It.Is<TimeSpan>(ts => ts == expectedRemainingTime)), Times.Once);
            billingPurchaseRepositoryMock
                .Verify(
                    x => x.UpdateAsync(It.Is<InAppBillingPurchase>(p =>
                        p.Id == billingPurchase.Id && p.State == PurchaseState.Failed)), Times.Once);
        }

        [Fact]
        public async Task HandlePendingPurchases_PendingPurchaseNotFound_ConsumesPurchase()
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
            billingPurchase.TransactionDateUtc = billingPurchase.TransactionDateUtc.AddMinutes(-6);
            var billingPurchaseInStore = CreateInAppBillingPurchase(PurchaseState.Purchased);
            billingPurchaseInStore.Id = "GPA";
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
            userSubscriptionServiceMock
                .Verify(x => x.UpdateRemainingTimeAsync(It.Is<TimeSpan>(ts => ts == expectedRemainingTime)), Times.Once);
            billingPurchaseRepositoryMock
                .Verify(
                    x => x.UpdateAsync(It.Is<InAppBillingPurchase>(p =>
                        p.Id == billingPurchase.Id && p.State == PurchaseState.Purchased)), Times.Once);
        }

        [Fact]
        public async Task HandlePendingPurchases_PendingPurchaseNotFound_ConsumePurchaseThrowsException_ThrowsNoPurchasesInStoreException()
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
            billingPurchase.TransactionDateUtc = billingPurchase.TransactionDateUtc.AddMinutes(-6);
            var billingPurchaseInStore = CreateInAppBillingPurchase(PurchaseState.Purchased);
            billingPurchaseInStore.Id = "GPA";
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
                .Throws(new Exception());
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
            await Assert.ThrowsAsync<PurchaseNotFoundException>(async () => await billingPurchaseService.HandlePendingPurchases());

            // Assert
            userSubscriptionServiceMock
                .Verify(x => x.UpdateRemainingTimeAsync(It.Is<TimeSpan>(ts => ts == expectedRemainingTime)), Times.Once);
            billingPurchaseRepositoryMock
                .Verify(
                    x => x.UpdateAsync(It.Is<InAppBillingPurchase>(p =>
                        p.Id == billingPurchase.Id && p.State == PurchaseState.Failed)), Times.Once);
        }

        [Fact]
        public async Task HandlePendingPurchases_PendingPurchaseNotFound_ConsumePurchaseReturnsFalse_ThrowsNoPurchasesInStoreException()
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
            billingPurchase.TransactionDateUtc = billingPurchase.TransactionDateUtc.AddMinutes(-6);
            var billingPurchaseInStore = CreateInAppBillingPurchase(PurchaseState.Purchased);
            billingPurchaseInStore.Id = "GPA";
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
                .ReturnsAsync(false);
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
            await Assert.ThrowsAsync<PurchaseNotFoundException>(async () => await billingPurchaseService.HandlePendingPurchases());

            // Assert
            userSubscriptionServiceMock
                .Verify(x => x.UpdateRemainingTimeAsync(It.Is<TimeSpan>(ts => ts == expectedRemainingTime)), Times.Once);
            billingPurchaseRepositoryMock
                .Verify(
                    x => x.UpdateAsync(It.Is<InAppBillingPurchase>(p =>
                        p.Id == billingPurchase.Id && p.State == PurchaseState.Failed)), Times.Once);
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
