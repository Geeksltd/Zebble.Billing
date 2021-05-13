namespace Zebble.Billing
{
    using Plugin.InAppBilling;

    static class EventArgsExtensions
    {
        public static SubscriptionRestoredEventArgs ToEventArgs(this Subscription subscription)
        {
            return new SubscriptionRestoredEventArgs
            {
                ProductId = subscription?.ProductId,
                SubscriptionDate = subscription?.SubscriptionDate,
                ExpirationDate = subscription?.ExpirationDate,
                CancellationDate = subscription?.CancellationDate
            };
        }

        public static SubscriptionPurchasedEventArgs ToEventArgs(this InAppBillingPurchase purchase)
        {
            return new SubscriptionPurchasedEventArgs
            {
                ProductId = purchase.ProductId,
                TransactionId = purchase.Id,
                TransactionDate = purchase.TransactionDateUtc,
                PurchaseToken = purchase.PurchaseToken
            };
        }
    }
}