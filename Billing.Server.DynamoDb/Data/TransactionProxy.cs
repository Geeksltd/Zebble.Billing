namespace Zebble.Billing
{
    using Amazon.DynamoDBv2.DataModel;

    [DynamoDBTable("Transactions")]
    public sealed class TransactionProxy : Transaction
    {
        [DynamoDBHashKey]
        public override string Id { get; set; }

        [DynamoDBGlobalSecondaryIndexHashKey("SubscriptionId-index")]
        public override string SubscriptionId { get; set; }

        public TransactionProxy() { }

        internal TransactionProxy(Transaction that) => this.CopyPropertiesFrom(that);
    }
}
