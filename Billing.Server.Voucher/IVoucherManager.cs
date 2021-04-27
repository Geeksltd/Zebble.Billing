namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;

    public interface IVoucherManager
    {
        Task<string> Generate(TimeSpan duration, string productId, string comments);
        Task<ApplyVoucherResult> Apply(string userId, string code);
    }
}
