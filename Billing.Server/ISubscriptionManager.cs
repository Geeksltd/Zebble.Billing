namespace Zebble.Billing
{
    using System.Threading.Tasks;

    public interface ISubscriptionManager
    {
        Task<Subscription[]> Refresh(string userId, string[] tokens);
        Task<long?> GetExpiry(string token);
        Task SavePurchase(string packageName, string json);
    }
}
