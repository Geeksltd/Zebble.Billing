namespace Zebble.Billing
{
    using System;

    public class SubscriptionPurchasedEventArgs : EventArgs
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public DateTime TransactionDateUtc { get; set; }
        public string PurchaseToken { get; set; }
    }
}
