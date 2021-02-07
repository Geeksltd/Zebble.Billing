namespace Zebble.Billing
{
    using System.Text.Json.Serialization;

    class AppStoreLatestReceiptInfo
    {
        /// <summary>
        /// The environment for which App Store generated the receipt.
        /// </summary>
        [JsonPropertyName("environment")]
        public AppStoreEnvironment Environment { get; set; }

        /// <summary>
        /// Theevent for which the App Store sent the notification.
        /// </summary>
        [JsonPropertyName("original_transaction_id")]
        public string OriginalTransactionId { get; set; }

    }
}
