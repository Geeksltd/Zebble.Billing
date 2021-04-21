namespace Zebble.Billing
{
    using System.Threading.Tasks;

    class VoucherRepository : IVoucherRepository
    {
        readonly VoucherDbContext Context;

        public VoucherRepository(VoucherDbContext context) => Context = context;

        public async Task<Voucher> GetByCode(string code)
        {
            return await Context.VoucherCodes.FirstOrDefault(code);
        }

        public async Task Update(Voucher voucher)
        {
            await Context.Vouchers.UpdateAsync(x => x.Id, new VoucherProxy(voucher));
        }
    }
}
