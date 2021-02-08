namespace Zebble.Billing
{
    using System.Threading.Tasks;

    public interface IVoucherRepository
    {
        Task<Voucher> GetByCode(string code);
        Task<Voucher> Update(Voucher voucher);
    }
}
