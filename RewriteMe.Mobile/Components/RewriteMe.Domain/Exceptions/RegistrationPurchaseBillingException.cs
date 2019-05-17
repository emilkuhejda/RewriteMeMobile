using System;
using Plugin.InAppBilling.Abstractions;

namespace RewriteMe.Domain.Exceptions
{
    public class RegistrationPurchaseBillingException : Exception
    {
        public RegistrationPurchaseBillingException()
        {
        }

        public RegistrationPurchaseBillingException(InAppBillingPurchase billingPurchase, string message, Exception innerException)
            : base(message, innerException)
        {
            BillingPurchase = billingPurchase;
        }

        public RegistrationPurchaseBillingException(string message)
            : base(message)
        {
        }

        public RegistrationPurchaseBillingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public InAppBillingPurchase BillingPurchase { get; }
    }
}
