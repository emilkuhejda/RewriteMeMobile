using System;
using Plugin.InAppBilling.Abstractions;

namespace RewriteMe.Domain.Exceptions
{
    public class PurchasePayloadNotValidException : Exception
    {
        public PurchasePayloadNotValidException()
        {
        }

        public PurchasePayloadNotValidException(InAppBillingPurchase billingPurchase)
        {
            BillingPurchase = billingPurchase;
        }

        public PurchasePayloadNotValidException(string message)
            : base(message)
        {
        }

        public PurchasePayloadNotValidException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public InAppBillingPurchase BillingPurchase { get; }
    }
}
