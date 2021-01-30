namespace Zebble.Billing
{
    using System.Threading.Tasks;

    class CafeBazaarSubscriptionProcessor : ISubscriptionProcessor
    {
        public SubscriptionPlatform Platform => SubscriptionPlatform.CafeBazaar;

        public Task<bool> Refresh()
        {
            return Task.FromResult(false);
        }
    }
}
