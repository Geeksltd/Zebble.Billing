namespace Zebble.Billing
{
    using System.Threading.Tasks;
    using Amazon.DynamoDBv2.DataModel;
    using Amazon.DynamoDBv2.DocumentModel;

    class VoucherRepository : IVoucherRepository
    {
        readonly VoucherDbContext Context;

        public VoucherRepository(VoucherDbContext context) => Context = context;

        public Task<Voucher> GetByCode(string code)
        {
            var condition = new ScanCondition(nameof(Voucher.Code), ScanOperator.Equal, code);
            return Context.FirstOrDefault<Voucher>(condition);
        }

        public async Task Update(Voucher voucher)
        {
            await Context.UpdateAsync(x => x.Id, voucher);
        }
    }
}
