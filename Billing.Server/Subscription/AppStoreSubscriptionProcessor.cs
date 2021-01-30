namespace Zebble.Billing
{
    using System.Threading.Tasks;

    class AppStoreSubscriptionProcessor : ISubscriptionProcessor
    {
        public SubscriptionPlatform Platform => SubscriptionPlatform.AppStore;

        public Task<bool> Refresh()
        {
            return Task.FromResult(false);
        }
    }
}
