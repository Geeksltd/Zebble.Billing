namespace Zebble.Billing
{
    using Amazon.DynamoDBv2.DataModel;
    using System;

    [DynamoDBTable("Vouchers")]
    sealed class VoucherProxy : Voucher, IBillingDynamoDbProxy
    {
        [DynamoDBHashKey]
        public override string Id { get; set; }

        [DynamoDBGlobalSecondaryIndexHashKey("Code-index")]
        public override string Code { get; set; }

        [DynamoDBGlobalSecondaryIndexHashKey("UserId-index")]
        public override string UserId { get; set; }

        [DynamoDBIgnore]
        public override TimeSpan Duration
        {
            get => TimeSpan.FromMilliseconds(DurationInMilliseconds);
            set => DurationInMilliseconds = (long)value.TotalMilliseconds;
        }

        public long DurationInMilliseconds { get; set; }

        public VoucherProxy() { }

        internal VoucherProxy(Voucher that) => this.CopyPropertiesFrom(that);
    }
}
