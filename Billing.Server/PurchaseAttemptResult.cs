namespace Zebble.Billing
{
    public partial class PurchaseAttemptResult
    {
        PurchaseAttemptResult(PurchaseAttemptStatus status) => Status = status;

        internal static PurchaseAttemptResult Failed = new(PurchaseAttemptStatus.Failed);

        internal static PurchaseAttemptResult UserMismatchedAndBlocked(string originUserId)
        {
            return new(PurchaseAttemptStatus.UserMismatchedAndBlocked)
            {
                OriginUserId = originUserId
            };
        }

        internal static PurchaseAttemptResult UserMismatchedAndReplaced(string originUserId)
        {
            return new(PurchaseAttemptStatus.UserMismatchedAndReplaced)
            {
                OriginUserId = originUserId
            };
        }

        internal static PurchaseAttemptResult Succeeded = new(PurchaseAttemptStatus.Succeeded);
    }
}
