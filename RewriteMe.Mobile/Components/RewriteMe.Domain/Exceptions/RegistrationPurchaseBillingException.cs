using System;

namespace RewriteMe.Domain.Exceptions
{
    public class RegistrationPurchaseBillingException : PurchaseBaseException
    {
        public RegistrationPurchaseBillingException()
        {
        }

        public RegistrationPurchaseBillingException(string message)
            : base(message)
        {
        }

        public RegistrationPurchaseBillingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public RegistrationPurchaseBillingException(string purchaseId, string productId, string message, Exception innerException)
            : base(purchaseId, productId, message, innerException)
        {
        }
    }
}
