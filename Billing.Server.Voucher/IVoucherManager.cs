namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;

    public interface IVoucherManager
    {
        Task<string> Generate(TimeSpan duration, string productId, string comments);
        Task Apply(string userId, string code);
    }
}
