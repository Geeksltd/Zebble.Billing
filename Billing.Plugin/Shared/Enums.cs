namespace Zebble.Billing
{
    public enum PurchaseResult
    {
        NotCompleted,
        GeneralError,
        BillingUnavailable,
        PaymentInvalid,
        PaymentNotAllowed,
        AppStoreUnavailable,
        Cancelled,
        WillBeActivated,
        Succeeded
    }
}
