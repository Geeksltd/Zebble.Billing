namespace Zebble.Billing
{
    using System.Threading.Tasks;

    public interface ISubscriptionManager
    {
        Task<PurchaseAttemptResult> PurchaseAttempt(string userId, string platform, string productId, string transactionId, string purchaseToken, bool replaceConfirmed);
        Task<Subscription> GetSubscriptionStatus(string userId);
    }
}