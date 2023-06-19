namespace Zebble.Billing
{
    using System;
    using System.Text.Json.Serialization;

    class HuaweiNotification
    {
        [JsonPropertyName("subNotification")]
        public HuaweiSubNotification SubNotification { get; set; }

        public string OriginalData { get; private set; }

        public HuaweiNotification WithOriginalData(string originalData)
        {
            OriginalData = originalData;
            return this;
        }

        public HuaweiEnvironment? Environment
            => SubNotification.StatusUpdateNotification.Environment;

        public string PurchaseToken
            => SubNotification.StatusUpdateNotification.PurchaseToken;

        public string ProductId
            => SubNotification.StatusUpdateNotification.ProductId;

        public bool AutoRenewStatus
            => SubNotification.StatusUpdateNotification.AutoRenewStatus;

        public DateTime? PurchaseTime
            => SubNotification.StatusUpdateNotification.LatestReceiptInfo.PurchaseTime;

        public DateTime? ExpirationDate
            => SubNotification.StatusUpdateNotification.LatestReceiptInfo.ExpirationDate;

        public DateTime? CancellationDate
            => SubNotification.StatusUpdateNotification.LatestReceiptInfo.CancellationDate;

        public SubscriptionInfoArgs ToArgs() => new()
        {
            ProductId = SubNotification.StatusUpdateNotification.ProductId,
            PurchaseToken = SubNotification.StatusUpdateNotification.PurchaseToken
        };
    }

    class HuaweiSubNotification
    {
        [JsonPropertyName("statusUpdateNotification")]
        public string StatusUpdateNotificationStr { get; set; }

        public HuaweiStatusUpdateNotification StatusUpdateNotification
            => StatusUpdateNotificationStr.FromJson<HuaweiStatusUpdateNotification>();
    }

    class HuaweiStatusUpdateNotification
    {
        [JsonPropertyName("environment")]
        [JsonConverter(typeof(EnumConverter<HuaweiEnvironment>))]
        public HuaweiEnvironment? Environment { get; set; }

        [JsonPropertyName("purchaseToken")]
        public string PurchaseToken { get; set; }

        [JsonPropertyName("latestReceiptInfo")]
        public string LatestReceiptInfoStr { get; set; }

        public HuaweiLatestReceiptInfo LatestReceiptInfo
            => LatestReceiptInfoStr.FromJson<HuaweiLatestReceiptInfo>();

        [JsonPropertyName("autoRenewStatus")]
        [JsonConverter(typeof(BooleanConverter))]
        public bool AutoRenewStatus { get; set; }

        [JsonPropertyName("productId")]
        public string ProductId { get; set; }
    }

    class HuaweiLatestReceiptInfo
    {
        [JsonPropertyName("purchaseTime")]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime? PurchaseTime { get; set; }

        [JsonPropertyName("expirationDate")]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime? ExpirationDate { get; set; }

        [JsonPropertyName("cancellationDate")]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime? CancellationDate { get; set; }
    }
}
