namespace Zebble.Billing
{
    public class SubscriptionInfoArgs
    {
        public string ProductId { get; set; }
        public string PurchaseToken { get; set; }
        public string ReceiptData { get; set; }
    }
}
