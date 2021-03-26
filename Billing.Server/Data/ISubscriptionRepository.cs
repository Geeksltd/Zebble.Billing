namespace Zebble.Billing
{
    using System.Threading.Tasks;

    public interface ISubscriptionRepository
    {
        Task<Subscription> GetByTransactionId(string transactionId);
        Task<Subscription> GetByPurchaseToken(string purchaseToken);
        Task<Subscription> GetMostUpdatedByUserId(string userId);
        Task<Subscription> AddSubscription(Subscription subscription);
        Task UpdateSubscription(Subscription subscription);
        Task<Transaction> AddTransaction(Transaction transaction);
        Task<string> GetOriginUserOfTransactionIds(string[] transactionIds);
    }
}
