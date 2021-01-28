namespace Zebble.Billing
{
    using System.Threading.Tasks;

    public interface ISubscriptionRepository
    {
        Task<Subscription> GetByPurchaseToken(string purchaseToken);
        Task<Subscription> GetMostUpdatedByUserId(string userId);
        Task<Subscription> Add(Subscription subscription);
        Task Update(Subscription subscription);
    }
}
