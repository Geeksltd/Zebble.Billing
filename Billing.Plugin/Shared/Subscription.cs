namespace Zebble.Billing
{
    using System;
    using Newtonsoft.Json;
    using Olive;

    public partial class Subscription
    {
        public string ProductId { get; set; }
        public string UserId { get; set; }

        public SubscriptionPlatform Platform { get; set; }
        [JsonIgnore]
        public string Token { get; set; }

        public DateTime DateSubscribed { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string OriginalTransactionId { get; set; }
        public DateTime LastUpdated { get; set; }

        public bool AutoRenews { get; set; }
        public bool IsCanceled { get; set; }

        public bool IsExpired => ExpiryDate < LocalTime.UtcToday;

        // TODO: remove
        [Obsolete]
        public string System
        {
            get { return Platform.ToString(); }
            set { Platform = value.To<SubscriptionPlatform>(); }
        }

        // TODO: remove
        [Obsolete]
        public DateTime PurchasedUtc
        {
            get { return DateSubscribed; }
            set { DateSubscribed = value; }
        }

        // TODO: remove
        [Obsolete]
        public DateTime ExpiryUtc
        {
            get { return ExpiryDate; }
            set { ExpiryDate = value; }
        }
    }
}
