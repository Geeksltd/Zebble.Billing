namespace Zebble.Billing
{
    using System;
    using Amazon.DynamoDBv2.DataModel;
    using Newtonsoft.Json;

    [DynamoDBTable("Transactions")]
    public class Transaction
    {
        public string TransactionId { get; set; }
        public DateTime Date { get; set; }

        public SubscriptionPlatform Platform { get; set; }

        public string SubscriptionId { get; set; }
        public string ProductId { get; set; }

        [JsonIgnore]
        public string State { get; set; }

        [JsonIgnore]
        public string Details { get; set; }
    }
}
