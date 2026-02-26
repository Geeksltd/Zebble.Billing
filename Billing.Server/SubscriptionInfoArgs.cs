namespace Zebble.Billing
{
    public class SubscriptionInfoArgs
    {
        public string PackageName { get; set; }
        public string ProductId { get; set; }
        public string SubscriptionId { get; set; }
        public string PurchaseToken { get; set; }
        public string OriginalTransactionId { get; set; }
    }
}
