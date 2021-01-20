namespace Zebble.Billing
{
    using System;

    public class SubscriptionRestoredEventArgs : EventArgs
    {
        public Product Product { get; set; }
        public DateTime SubscriptionExpiry { get; set; }
    }

    public class PurchaseRecognizedEventArgs : EventArgs
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public DateTime TransactionDateUtc { get; set; }
        public string PurchaseToken { get; set; }
    }

    public class VoucherAppliedEventArgs : EventArgs
    {
        public string VoucherCode { get; set; }
    }
}
