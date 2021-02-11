namespace Zebble.Billing
{
    using Plugin.InAppBilling;
    using System.Threading.Tasks;

    static class EventArgsExtensions
    {
        public static async Task<SubscriptionRestoredEventArgs<T>> ToEventArgs<T>(this Subscription subscription) where T : Product
        {
            return new SubscriptionRestoredEventArgs<T>
            {
                Product = await BillingContext<T>.Current.ProductProvider.GetById(subscription.ProductId),
                Subscription = subscription
            };
        }

        public static async Task<SubscriptionPurchasedEventArgs<T>> ToEventArgs<T>(this InAppBillingPurchase purchase) where T : Product
        {
            return new SubscriptionPurchasedEventArgs<T>
            {
                Id = purchase.Id,
                Product = await BillingContext<T>.Current.ProductProvider.GetById(purchase.ProductId),
                TransactionDateUtc = purchase.TransactionDateUtc,
                PurchaseToken = purchase.PurchaseToken
            };
        }
    }
}