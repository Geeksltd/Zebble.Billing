namespace Zebble.Billing
{
    using System;
    using System.Text.Json.Serialization;

    class HuaweiNotification
    {
        /// <summary>
        /// The same value as the shared secret you submit in the password field of the requestBody when validating receipts.
        /// </summary>
        [JsonPropertyName("password")]
        public string SharedSecret { get; set; }

        /// <summary>
        /// The type that describes the in-app purchase event for which the App Store sent the notification.
        /// </summary>
        [JsonPropertyName("notification_type")]
        [JsonConverter(typeof(EnumConverter<HuaweiNotificationType>))]
        public HuaweiNotificationType Type { get; set; }

        /// <summary>
        /// The current renewal status for an auto-renewable subscription product.
        /// </summary>
        [JsonPropertyName("auto_renew_status")]
        [JsonConverter(typeof(NullableBooleanConverter))]
        public bool? AutoRenewStatus { get; set; }

        public string ProductId { get; set; }

        public string PurchaseToken { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public DateTime? CancellationDate { get; set; }

        /// <summary>
        /// Original notification json
        /// </summary>
        public string OriginalData { get; private set; }

        public HuaweiNotification WithOriginalData(string originalData)
        {
            OriginalData = originalData;
            return this;
        }

        public SubscriptionInfoArgs ToArgs() => new()
        {
            ProductId = ProductId,
            PurchaseToken = PurchaseToken
        };
    }
}
