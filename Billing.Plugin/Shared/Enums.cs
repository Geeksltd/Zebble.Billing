namespace Zebble.Billing
{
    public enum PurchaseResult
    {
        BillingUnavailable,
        NotCompleted,
        GeneralError,
        PaymentInvalid,
        PaymentNotAllowed,
        AppStoreUnavailable,
        UserMismatched,
        AlreadySubscribed,
        UserCancelled,
        WillBeActivated,
        Succeeded
    }
}
