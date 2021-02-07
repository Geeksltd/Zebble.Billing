namespace Zebble.Billing
{
    using System.Text.Json.Serialization;

    class AppStoreUnifiedReceipt
    {
        /// <summary>
        /// The environment for which App Store generated the receipt.
        /// </summary>
        [JsonPropertyName("environment")]
        public AppStoreEnvironment Environment { get; set; }

        /// <summary>
        /// The latest Base64-encoded app receipt.
        /// </summary>
        [JsonPropertyName("latest_receipt")]
        public string LatestReceipt { get; set; }

        /// <summary>
        /// An array that contains the latest 100 in-app purchase transactions of the decoded value in LatestReceipt. This array excludes transactions for consumable products your app has marked as finished.
        /// </summary>
        [JsonPropertyName("latest_receipt_info")]
        public AppStoreLatestReceiptInfo[] LatestReceiptInfo { get; set; }
    }
}
