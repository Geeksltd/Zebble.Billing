namespace Zebble.Billing
{
    static partial class EntityExtensions
    {
        internal static bool RequiresStoreUpdate(this Subscription @this) => @this.IsExpired() || @this.IsCanceled();

        public static SubscriptionInfoArgs ToArgs(this Subscription @this)
        {
            return new SubscriptionInfoArgs
            {
                UserId = @this.UserId,
                ProductId = @this.ProductId,
                PurchaseToken = @this.PurchaseToken,
                ReceiptData = @this.ReceiptData
            };
        }
    }
}
