namespace Zebble.Billing
{
    using System;
    using Olive;

    public partial class Subscription
    {
        public string ProductId { get; set; }
        public string UserId { get; set; }

        public SubscriptionPlatform Platform { get; set; }

        public string PurchaseToken { get; set; }

        public DateTime? DateSubscribed { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? CancellationDate { get; set; }
        public DateTime? LastUpdated { get; set; }

        public bool? AutoRenews { get; set; }

        [Obsolete("Use Platform")]
        public string System
        {
            get { return Platform.ToString(); }
            set { Platform = value.To<SubscriptionPlatform>(); }
        }

        [Obsolete("Use DateSubscribed")]
        public DateTime PurchasedUtc
        {
            get { return DateSubscribed ?? DateTime.MinValue; }
            set { DateSubscribed = value; }
        }

        [Obsolete("Use ExpiryDate")]
        public DateTime ExpiryUtc
        {
            get { return ExpiryDate ?? DateTime.MinValue; }
            set { ExpiryDate = value; }
        }
    }
}
