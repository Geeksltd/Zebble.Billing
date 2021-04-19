namespace Zebble.Billing
{
    public enum PurchaseResult
    {
        BillingUnavailable,
        NotCompleted,
        PaymentInvalid,
        PaymentNotAllowed,
        AppStoreUnavailable,
        UserMismatched,
        Unknown,
        AlreadySubscribed,
        UserCancelled,
        WillBeActivated,
        Succeeded
    }

    public enum ProductType { Subscription, InAppPurchase }
}
