namespace Zebble.Billing
{
    using System.Linq;
    using System.Threading.Tasks;
    using Olive;

    class SubscriptionRepository : ISubscriptionRepository
    {
        readonly SubscriptionDbContext Context;

        public SubscriptionRepository(SubscriptionDbContext context) => Context = context;

        public async Task<Subscription> GetByTransactionId(string transactionId)
        {
            return (await Context.SubscriptionTransactions.All(transactionId))
                                                          .OrderBy(x => x.SubscriptionDate)
                                                          .LastOrDefault();
        }

        public async Task<Subscription> GetByPurchaseToken(string purchaseToken)
        {
            var hash = purchaseToken.ToSimplifiedSHA1Hash();
            return await Context.SubscriptionPurchaseTokenHashes.FirstOrDefault(hash);
        }

        public async Task<Subscription[]> GetAll(string userId)
        {
            return await Context.SubscriptionUsers.All(userId);
        }

        public async Task<Subscription> AddSubscription(Subscription subscription)
        {
            await Context.Subscriptions.AddAsync(new SubscriptionProxy(subscription));

            return subscription;
        }

        public Task UpdateSubscription(Subscription subscription)
        {
            return Context.Subscriptions.UpdateAsync(x => x.Id, new SubscriptionProxy(subscription));
        }

        public Task UpdateSubscriptions(Subscription[] subscriptions)
        {
            return Task.WhenAll(subscriptions.DoAsync(UpdateSubscription));
        }

        public async Task<Transaction> AddTransaction(Transaction transaction)
        {
            await Context.Transactions.AddAsync(new TransactionProxy(transaction));

            return transaction;
        }

        public async Task<string> GetOriginUserOfTransactionId(string transactionId)
        {
            var subscriptions = await Context.SubscriptionTransactions.All(transactionId);

            return subscriptions.Where(x => !x.IsCanceled())
                                .Select(x => x.UserId)
                                .FirstOrDefault(x => x.HasValue());
        }

        public async Task<Subscription[]> GetAllWithTransactionIdNotOwnedBy(string userId, string transactionId)
        {
            var subscriptions = await Context.SubscriptionTransactions.All(transactionId);

            return subscriptions.Except(x => x.UserId == userId).ToArray();
        }
    }
}
