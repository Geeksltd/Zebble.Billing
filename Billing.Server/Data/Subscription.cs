namespace Zebble.Billing
{
    using System;

    public partial class Subscription
    {
        public virtual string Id { get; set; }

        public virtual string UserId { get; set; }

        public string Platform { get; set; }

        public virtual string TransactionId { get; set; }
        public string ReceiptData { get; set; }
        public DateTime? TransactionDate { get; set; }
        public virtual string PurchaseToken { get; set; }

        public DateTime? LastUpdate { get; set; }

        public bool? AutoRenews { get; set; }
    }
}
