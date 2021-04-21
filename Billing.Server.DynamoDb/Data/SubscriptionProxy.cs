namespace Zebble.Billing
{
    using Amazon.DynamoDBv2.DataModel;

    [DynamoDBTable("Subscriptions")]
    public sealed class SubscriptionProxy : Subscription, IBillingDynamoDbProxy
    {
        [DynamoDBHashKey]
        public override string Id { get; set; }

        [DynamoDBGlobalSecondaryIndexHashKey("UserId-index")]
        public override string UserId { get; set; }

        [DynamoDBGlobalSecondaryIndexHashKey("TransactionId-index")]
        public override string TransactionId { get; set; }

        [DynamoDBGlobalSecondaryIndexHashKey("PurchaseToken-index")]
        public override string PurchaseToken { get; set; }

        public SubscriptionProxy() { }

        internal SubscriptionProxy(Subscription that) => this.CopyPropertiesFrom(that);
    }
}
