namespace Zebble.Billing
{
    using Amazon.DynamoDBv2;
    using Amazon.DynamoDBv2.DataModel;

    class SubscriptionDbContext : DynamoDBContext
    {
        public SubscriptionDbContext(IAmazonDynamoDB client) : base(client) { }
    }
}
