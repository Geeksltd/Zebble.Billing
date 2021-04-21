namespace Zebble.Billing
{
    using Amazon.DynamoDBv2;

    class VoucherDbContext : BillingDynamoDbContext
    {
        public VoucherDbContext(IAmazonDynamoDB client) : base(client) { }

        public DbTable<VoucherProxy> Vouchers => Table<VoucherProxy>();
        public DbIndex<VoucherProxy> VoucherCodes => Index<VoucherProxy>("Code-index");
        public DbIndex<VoucherProxy> VoucherUsers => Index<VoucherProxy>("UserId-index");
    }
}
