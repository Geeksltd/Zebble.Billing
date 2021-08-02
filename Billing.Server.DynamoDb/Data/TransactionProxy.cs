namespace Zebble.Billing
{
    using Amazon.DynamoDBv2.DataModel;

    [DynamoDBTable("Transactions")]
    sealed class TransactionProxy : Transaction, IBillingDynamoDbProxy
    {
        [DynamoDBHashKey]
        public override string Id { get; set; }

        [DynamoDBGlobalSecondaryIndexHashKey("SubscriptionId-index")]
        public override string SubscriptionId { get; set; }

        public TransactionProxy() { }

        internal TransactionProxy(Transaction that) => this.CopyPropertiesFrom(that);
    }
}
