namespace Zebble.Billing
{
    using Amazon.DynamoDBv2.DataModel;

    [DynamoDBTable("Vouchers")]
    public sealed class VoucherProxy : Voucher, IBillingDynamoDbProxy
    {
        [DynamoDBHashKey]
        public override string Id { get; set; }

        [DynamoDBGlobalSecondaryIndexHashKey("Code-index")]
        public override string Code { get; set; }

        [DynamoDBGlobalSecondaryIndexHashKey("UserId-index")]
        public override string UserId { get; set; }

        public VoucherProxy() { }

        internal VoucherProxy(Voucher that) => this.CopyPropertiesFrom(that);
    }
}
