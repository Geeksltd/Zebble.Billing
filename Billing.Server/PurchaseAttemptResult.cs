namespace Zebble.Billing
{
    public partial class PurchaseAttemptResult
    {
        PurchaseAttemptResult(PurchaseAttemptStatus status) => Status = status;

        internal static PurchaseAttemptResult Failed = new(PurchaseAttemptStatus.Failed);

        internal static PurchaseAttemptResult UserMismatched(string originUserId, string newUserId = null)
        {
            return new(PurchaseAttemptStatus.UserMismatched)
            {
                OriginUserId = originUserId,
                NewUserId = newUserId,
            };
        }

        internal static PurchaseAttemptResult Succeeded(string originUserId = null)
        {
            return new(PurchaseAttemptStatus.Succeeded)
            {
                OriginUserId = originUserId,
            };
        }
    }
}
