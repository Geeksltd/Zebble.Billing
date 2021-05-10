namespace Zebble.Billing
{
    using Amazon.DynamoDBv2;

    class SubscriptionDbContext : BillingDynamoDbContext
    {
        public SubscriptionDbContext(IAmazonDynamoDB client) : base(client) { }

        public DbTable<SubscriptionProxy> Subscriptions => Table<SubscriptionProxy>();
        public DbIndex<SubscriptionProxy> SubscriptionUsers => Index<SubscriptionProxy>("UserId-index");
        public DbIndex<SubscriptionProxy> SubscriptionTransactions => Index<SubscriptionProxy>("TransactionId-index");
        public DbIndex<SubscriptionProxy> SubscriptionPurchaseTokenHashes => Index<SubscriptionProxy>("PurchaseTokenHash-index");

        public DbTable<TransactionProxy> Transactions => Table<TransactionProxy>();
        public DbIndex<TransactionProxy> TransactionSubscriptions => Index<TransactionProxy>("SubscriptionId-index");
    }
}
