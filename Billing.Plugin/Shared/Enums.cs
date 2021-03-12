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
        UserCancelled,
        WillBeActivated,
        Succeeded
    }
}
