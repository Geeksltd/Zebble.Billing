namespace Zebble.Billing
{
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Olive;

    class VoucherRepository : IVoucherRepository
    {
        readonly VoucherDbContext Context;

        public VoucherRepository(VoucherDbContext context) => Context = context;

        public Task<Voucher> GetByCode(string code)
        {
            return Context.Vouchers.SingleOrDefaultAsync(x => x.Code == code);
        }

        public async Task<Voucher> Update(Voucher voucher)
        {
            await Context.Vouchers.AddAsync(voucher);
            await Context.SaveChangesAsync();

            return voucher;
        }
    }
}
