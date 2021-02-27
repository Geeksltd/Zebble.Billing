namespace Zebble.Billing
{
    using System;
    using System.Linq;
    using System.Text.Json.Serialization;

    class AppStoreNotification
    {
        /// <summary>
        /// The same value as the shared secret you submit in the password field of the requestBody when validating receipts.
        /// </summary>
        [JsonPropertyName("password")]
        public string SharedSecret { get; set; }

        /// <summary>
        /// The environment for which App Store generated the receipt.
        /// </summary>
        public AppStoreEnvironment? Environment => UnifiedReceipt?.Environment;

        /// <summary>
        /// The type that describes the in-app purchase event for which the App Store sent the notification.
        /// </summary>
        [JsonPropertyName("notification_type")]
        [JsonConverter(typeof(EnumConverter<AppStoreNotificationType>))]
        public AppStoreNotificationType Type { get; set; }

        /// <summary>
        /// The current renewal status for an auto-renewable subscription product.
        /// </summary>
        [JsonPropertyName("auto_renew_status")]
        [JsonConverter(typeof(NullableBooleanConverter))]
        public bool? AutoRenewStatus { get; set; }

        public string ProductId => CandidateReceiptInfo?.ProductId;

        public string PurchaseToken => UnifiedReceipt?.LatestReceipt;

        public DateTime? PurchaseDate => CandidateReceiptInfo?.PurchaseDate;

        public DateTime? CancellationDate => CandidateReceiptInfo?.CancellationDate;

        public DateTime? ExpirationDate => CandidateReceiptInfo?.ExpirationDate;

        public bool? IsInBillingRetryPeriod => CandidateRenewalInfo?.IsInBillingRetryPeriod;

        public DateTime? GracePeriodExpirationDate => CandidateRenewalInfo?.GracePeriodExpirationDate;

        /// <summary>
        /// An object that contains information about the most-recent, in-app purchase transactions for the app.
        /// </summary>
        [JsonPropertyName("unified_receipt")]
        public AppStoreUnifiedReceipt UnifiedReceipt { get; set; }

        /// <summary>
        /// Original notification json
        /// </summary>
        public string OriginalData { get; private set; }

        public AppStoreNotification WithOriginalData(string originalData)
        {
            OriginalData = originalData;
            return this;
        }

        AppStoreLatestReceiptInfo CandidateReceiptInfo => UnifiedReceipt?.LatestReceiptInfo?.FirstOrDefault();

        AppStorePendingRenewaInfo CandidateRenewalInfo => UnifiedReceipt?.PendingRenewalInfo?.FirstOrDefault();
    }
}
