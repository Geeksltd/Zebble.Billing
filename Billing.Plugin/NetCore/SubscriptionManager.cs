namespace Zebble.Billing
{
    using System.Threading.Tasks;

    public class SubscriptionManager : ISubscriptionManager
    {
        public Task<Subscription[]> Refresh(string userId, string[] tokens)
        {
            throw new System.NotImplementedException();
        }

        public Task<long?> GetExpiry(string token)
        {
            throw new System.NotImplementedException();
        }

        public Task ProcessAppleNotification(string json)
        {
            throw new System.NotImplementedException();
        }
    }
}
