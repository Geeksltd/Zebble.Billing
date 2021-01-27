namespace Zebble.Billing
{
    using System.Linq;
    using System.Threading.Tasks;

    public class SubscriptionManager : ISubscriptionManager
    {
        public Task<Subscription[]> Refresh(string userId, string[] tokens)
        {
            return Task.FromResult(new Subscription[0]);
        }

        public Task<long?> GetExpiry(string token)
        {
            return Task.FromResult<long?>(null);
        }

        public Task SavePurchase(string packageName, string json)
        {
            return Task.CompletedTask;
        }
    }
}
