namespace Zebble.Billing
{
    static partial class EntityExtensions
    {
        internal static SubscriptionInfoArgs ToArgs(this Subscription @this)
        {
            return new SubscriptionInfoArgs
            {
                UserId = @this.UserId,
                ProductId = @this.ProductId,
                PurchaseToken = @this.PurchaseToken
            };
        }

        internal static SubscriptionInfoArgs ToArgs(this AppPurchaseAttemptModel @this)
        {
            return new SubscriptionInfoArgs
            {
                UserId = @this.UserId,
                ProductId = @this.ProductId,
                PurchaseToken = @this.PurchaseToken
            };
        }
    }
}
