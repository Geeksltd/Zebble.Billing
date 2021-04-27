namespace Zebble.Billing
{
    class VerifyPurchaseResult
    {
        public PurchaseVerificationStatus Status { get; set; }

        internal static VerifyPurchaseResult From(PurchaseVerificationStatus status) => new() { Status = status };

        internal static VerifyPurchaseResult Succeeded() => new() { Status = PurchaseVerificationStatus.Verified };
    }
}
