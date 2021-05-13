namespace Zebble.Billing
{
    public partial class PurchaseAttemptResult
    {
        PurchaseAttemptResult(PurchaseAttemptStatus status) => Status = status;

        internal static PurchaseAttemptResult Failed = new(PurchaseAttemptStatus.Failed);

        internal static PurchaseAttemptResult UserMismatched = new(PurchaseAttemptStatus.UserMismatched);

        internal static PurchaseAttemptResult Succeeded = new(PurchaseAttemptStatus.Succeeded);
    }
}
