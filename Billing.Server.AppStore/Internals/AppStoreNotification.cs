namespace Zebble.Billing
{
    using System.Text.Json.Serialization;

    class AppStoreNotification
    {
        /// <summary>
        /// The same value as the shared secret you submit in the password field of the requestBody when validating receipts.
        /// </summary>
        [JsonPropertyName("password")]
        public string SharedSecret { get; set; }

        /// <summary>
        /// The current renewal status for an auto-renewable subscription product. Note that these values are different from those of the auto_renew_status in the receipt
        /// </summary>
        [JsonPropertyName("auto_renew_status")]
        public bool AutoRenew { get; set; }

        /// <summary>
        /// The environment for which App Store generated the receipt.
        /// </summary>
        public AppStoreEnvironment Environment => UnifiedReceipt.Environment;

        /// <summary>
        /// The type that describes the in-app purchase event for which the App Store sent the notification.
        /// </summary>
        [JsonPropertyName("notification_type")]
        public AppStoreNotificationType Type { get; set; }

        /// <summary>
        /// An object that contains information about the most-recent, in-app purchase transactions for the app.
        /// </summary>
        [JsonPropertyName("unified_receipt")]
        public AppStoreUnifiedReceipt UnifiedReceipt { get; set; }
    }
}
