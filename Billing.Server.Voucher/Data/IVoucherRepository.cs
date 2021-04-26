namespace Zebble.Billing
{
    using System.Threading.Tasks;

    public interface IVoucherRepository
    {
        Task<Voucher> GetByCode(string code);
        Task<Voucher> Add(Voucher voucher);
        Task Update(Voucher voucher);
    }
}
