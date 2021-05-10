namespace Zebble.Billing
{
    using Amazon.DynamoDBv2.DataModel;
    using Olive;

    [DynamoDBTable("Subscriptions")]
    public sealed class SubscriptionProxy : Subscription, IBillingDynamoDbProxy
    {
        [DynamoDBHashKey]
        public override string Id { get; set; }

        [DynamoDBGlobalSecondaryIndexHashKey("UserId-index")]
        public override string UserId { get; set; }

        [DynamoDBGlobalSecondaryIndexHashKey("TransactionId-index")]
        public override string TransactionId { get; set; }

        [DynamoDBGlobalSecondaryIndexHashKey("PurchaseTokenHash-index")]
        public string PurchaseTokenHash { get; set; }

        public SubscriptionProxy() { }

        internal SubscriptionProxy(Subscription that)
        {
            this.CopyPropertiesFrom(that);
            PurchaseTokenHash = PurchaseToken?.ToSimplifiedSHA1Hash();
        }
    }
}
