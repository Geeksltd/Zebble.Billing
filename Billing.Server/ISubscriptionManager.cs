namespace Zebble.Billing
{
    using System.Threading.Tasks;

    public interface ISubscriptionManager
    {
        Task InitiatePurchase(string productId, string userId, SubscriptionPlatform platform, string purchaseToken);
        Task<Subscription[]> RefreshSubscriptions(string userId, string[] purchaseTokens);
    }
}
