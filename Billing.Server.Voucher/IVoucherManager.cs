namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;

    public interface IVoucherManager
    {
        Task<string> Generate(TimeSpan duration, string productId, string comments, string discountCode);
        Task<ApplyVoucherResult> Apply(string userId, string code, DateTime? activationDate = null);
    }
}
