namespace Zebble.Billing
{
    using System;
    using Newtonsoft.Json;

    public partial class Subscription
    {
        public string ProductId { get; set; }
        public string UserId { get; set; }

        public SubscriptionStatus Platform { get; set; }
        [JsonIgnore]
        public string Token { get; set; }

        public DateTime DateSubscribed { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string OriginalTransactionId { get; set; }
        public DateTime LastUpdated { get; set; }

        public bool AutoRenews { get; set; }
        public bool IsCanceled { get; set; }
    }
}
