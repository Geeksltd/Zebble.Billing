namespace Zebble.Billing;

public partial record PurchaseAttemptResult
{
    internal static PurchaseAttemptResult Failed(SubscriptionInfo subscription)
    {
        return new(
            Status: PurchaseAttemptStatus.Failed,
            ProductId: subscription.ProductId,
            SubscriptionDate: subscription.SubscriptionDate?.ToString("o"),
            ExpirationDate: subscription.ExpirationDate?.ToString("o")
        );
    }

    internal static PurchaseAttemptResult UserMismatched(SubscriptionInfo subscription, string originUserId, string newUserId = null)
    {
        return new(
            Status: PurchaseAttemptStatus.UserMismatched,
            ProductId: subscription.ProductId,
            SubscriptionDate: subscription.SubscriptionDate?.ToString("o"),
            ExpirationDate: subscription.ExpirationDate?.ToString("o"),
            OriginUserId: originUserId,
            NewUserId: newUserId
        );
    }

    internal static PurchaseAttemptResult Succeeded(SubscriptionInfo subscription, string originUserId = null)
    {
        return new(
            Status: PurchaseAttemptStatus.Succeeded,
            ProductId: subscription.ProductId,
            SubscriptionDate: subscription.SubscriptionDate?.ToString("o"),
            ExpirationDate: subscription.ExpirationDate?.ToString("o"),
            OriginUserId: originUserId
        );
    }
}
