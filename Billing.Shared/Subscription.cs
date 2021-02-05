namespace Zebble.Billing
{
    using System;

    public partial class Subscription
    {
        public string SubscriptionId { get; set; }

        public string ProductId { get; set; }
        public string UserId { get; set; }

        public string Platform { get; set; }

        public string PurchaseToken { get; set; }
        public string OriginalTransactionId { get; set; }

        public DateTime? DateSubscribed { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? CancellationDate { get; set; }
        public DateTime? LastUpdated { get; set; }

        public bool? AutoRenews { get; set; }

        [Obsolete("Use Platform", true)]
        public string System
        {
            get { return Platform; }
            set { Platform = value; }
        }

        [Obsolete("Use DateSubscribed", true)]
        public DateTime PurchasedUtc
        {
            get { return DateSubscribed ?? DateTime.MinValue; }
            set { DateSubscribed = value; }
        }

        [Obsolete("Use ExpiryDate", true)]
        public DateTime ExpiryUtc
        {
            get { return ExpiryDate ?? DateTime.MinValue; }
            set { ExpiryDate = value; }
        }

        [Obsolete("Use PurchaseToken", true)]
        public string Token
        {
            get { return PurchaseToken; }
            set { PurchaseToken = value; }
        }
    }
}
