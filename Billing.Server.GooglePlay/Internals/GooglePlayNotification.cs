namespace Zebble.Billing
{
    using System;
    using System.Text.Json.Serialization;
    using Olive;

    class GooglePlayNotification
    {
        public DateTime? EventTime { get; set; }
        public string ProductId { get; set; }
        public string PurchaseToken { get; set; }
        public GooglePlaySubscriptionState State { get; set; }

        public string OriginalData { get; set; }

        public bool IsTest => PurchaseToken.IsEmpty();

        public class UnderlayingType
        {
            [JsonPropertyName("eventTimeMillis")]
            [JsonConverter(typeof(StringToLongConverter))]
            public long? EventTimeMillis { get; set; }

            [JsonPropertyName("subscriptionNotification")]
            public Subs SubscriptionNotification { get; set; }

            public class Subs
            {
                [JsonPropertyName("notificationType")]
                public int NotificationType { get; set; }

                [JsonPropertyName("purchaseToken")]
                public string PurchaseToken { get; set; }

                [JsonPropertyName("subscriptionId")]
                public string SubscriptionId { get; set; }
            }

            public GooglePlayNotification ToNotification(string originalData)
            {
                return new GooglePlayNotification
                {
                    EventTime = EventTimeMillis?.ToDateTime(),
                    ProductId = SubscriptionNotification?.SubscriptionId,
                    PurchaseToken = SubscriptionNotification?.PurchaseToken,
                    State = (GooglePlaySubscriptionState)SubscriptionNotification.NotificationType,
                    OriginalData = originalData
                };
            }
        }

        public SubscriptionInfoArgs ToArgs()
        {
            return new SubscriptionInfoArgs
            {
                ProductId = ProductId,
                PurchaseToken = PurchaseToken
            };
        }
    }
}
