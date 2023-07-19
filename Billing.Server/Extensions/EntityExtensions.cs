namespace Zebble.Billing
{
    static partial class EntityExtensions
    {
        internal static SubscriptionInfoArgs ToArgs(this Subscription @this)
        {
            return new SubscriptionInfoArgs
            {
                ProductId = @this.ProductId,
                SubscriptionId = @this.SubscriptionId,
                PurchaseToken = @this.PurchaseToken
            };
        }
    }
}
