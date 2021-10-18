namespace Zebble.Billing
{
    using System.Threading.Tasks;

    public interface ISubscriptionRepository
    {
        Task<Subscription[]> GetAll(string userId);
        Task<Subscription> AddSubscription(Subscription subscription);
        Task UpdateSubscription(Subscription subscription);
        Task UpdateSubscriptions(Subscription[] subscriptions);
        Task<Transaction> AddTransaction(Transaction transaction);
        Task<Subscription[]> GetAllWithTransactionId(string transactionId);
    }
}
