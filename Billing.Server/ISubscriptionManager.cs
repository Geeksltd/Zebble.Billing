namespace Zebble.Billing
{
    using System.Threading.Tasks;

    public interface ISubscriptionManager
    {
        Task InitiatePurchase(string productId, string userId, string platform, string purchaseToken);
        Task<Subscription> GetSubscriptionStatus(string userId);
    }
}
