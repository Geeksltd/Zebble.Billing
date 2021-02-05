namespace Zebble.Billing
{
    using System.Text.Json.Serialization;

    class AppStoreServerNotification
    {
        /// <summary>
        /// The same value as the shared secret you submit in the password field of the requestBody when validating receipts.
        /// </summary>
        [JsonPropertyName("password")]
        public string Password { get; set; }

        /// <summary>
        /// The current renewal status for an auto-renewable subscription product. Note that these values are different from those of the auto_renew_status in the receipt
        /// </summary>
        [JsonPropertyName("auto_renew_status")]
        public bool AutoRenew { get; set; }

        /// <summary>
        /// The environment for which App Store generated the receipt.
        /// </summary>
        [JsonPropertyName("environment")]
        public AppStoreServerEnvironment Environment { get; set; }
    }
}
