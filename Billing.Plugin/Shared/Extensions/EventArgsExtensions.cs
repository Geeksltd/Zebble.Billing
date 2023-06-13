namespace Zebble.Billing
{
    static partial class EventArgsExtensions
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
    }
}