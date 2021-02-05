namespace Zebble.Billing
{
    using System.Threading.Tasks;

    public interface IPlatformProvider
    {
        public string Platform { get; }
        Task<Subscription> GetUpToDateInfo(string productId, string purchaseToken);
    }
}
