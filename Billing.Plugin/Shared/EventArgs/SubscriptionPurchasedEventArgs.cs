namespace Zebble.Billing
{
    using System;

    public class SubscriptionPurchasedEventArgs : EventArgs
    {
        public string ProductId { get; set; }
        public string SubscriptionId { get; set; }
        public string TransactionId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string PurchaseToken { get; set; }
        public bool ReplaceConfirmed { get; set; }
    }
}
