namespace Zebble.Billing
{
    public enum PurchaseResult
    {
        BillingUnavailable,
        NotCompleted,
        PaymentInvalid,
        PaymentNotAllowed,
        AppStoreUnavailable,
        UserMismatchedAndBlocked,
        UserMismatchedAndReplaced,
        Unknown,
        AlreadySubscribed,
        UserCancelled,
        WillBeActivated,
        Succeeded
    }
}
