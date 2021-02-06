namespace Zebble.Billing
{
    using System.Threading.Tasks;

    class AppStoreConnector : IStoreConnector
    {
        public Task<Subscription> GetUpToDateInfo(string productId, string purchaseToken)
        {
            return Task.FromResult<Subscription>(null);
        }
    }
}
