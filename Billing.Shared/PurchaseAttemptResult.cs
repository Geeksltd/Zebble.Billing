namespace Zebble.Billing
{
    public partial class PurchaseAttemptResult
    {
        public PurchaseAttemptStatus Status { get; set; }
        public string OriginUserId { get; set; }
        public string NewUserId { get; set; }
    }
}
