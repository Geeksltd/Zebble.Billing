namespace Zebble.Billing
{
    using Amazon.DynamoDBv2;
    using Amazon.DynamoDBv2.DataModel;
    
    class VoucherDbContext : DynamoDBContext
    {
        public VoucherDbContext(IAmazonDynamoDB client) : base(client) { }
    }
}
