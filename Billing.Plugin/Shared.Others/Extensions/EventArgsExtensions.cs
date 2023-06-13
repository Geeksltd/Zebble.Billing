namespace Zebble.Billing
{
    using Plugin.InAppBilling;

    static partial class EventArgsExtensions
    {
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