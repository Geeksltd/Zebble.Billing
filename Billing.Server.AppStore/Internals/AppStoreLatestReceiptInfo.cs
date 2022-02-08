namespace Zebble.Billing
{
    using System;
    using System.Text.Json.Serialization;

    class AppStoreLatestReceiptInfo
    {
        /// <summary>
        /// The environment for which App Store generated the receipt.
        /// </summary>
        [JsonPropertyName("environment")]
        public AppStoreEnvironment Environment { get; set; }

        /// <summary>
        /// The unique identifier of the product purchased. You provide this value when creating the product in App Store Connect, and it corresponds to the productIdentifier property of the SKPayment object stored in the transaction's payment property.
        /// </summary>
        [JsonPropertyName("product_id")]
        public string ProductId { get; set; }

        /// <summary>
        /// The transaction identifier of the original purchase.
        /// </summary>
        [JsonPropertyName("original_transaction_id")]
        public string OriginalTransactionId { get; set; }

        [JsonPropertyName("transaction_id")]
        public string TransactionId { get; set; }

        [JsonPropertyName("original_purchase_date_ms")]
        public string OriginalPurchaseDateMs { get; set; }

        [JsonPropertyName("purchase_date_ms")]
        public string PurchaseDateMs { get; set; }

        /// <summary>
        /// The time when the App Store charged the user's account for a subscription purchase or renewal after a lapse.
        /// </summary>
        public DateTime? PurchaseDate => DateTimeConverter.Convert(PurchaseDateMs);

        [JsonPropertyName("expires_date_ms")]
        public string ExpiresDateMs { get; set; }

        /// <summary>
        /// The time when a subscription expires or when it will renew.
        /// </summary>
        public DateTime? ExpiresDate => DateTimeConverter.Convert(ExpiresDateMs);

        /// <summary>
        /// The time when Apple customer support canceled a transaction, or the time when the user upgraded an auto-renewable subscription plan.
        /// </summary>
        [JsonPropertyName("cancellation_date_ms")]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime? CancellationDate { get; set; }
    }
}
