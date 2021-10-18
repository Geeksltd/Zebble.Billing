namespace Zebble.Billing
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Olive;

    class SubscriptionRepository : ISubscriptionRepository
    {
        readonly SubscriptionDbContext Context;

        public SubscriptionRepository(SubscriptionDbContext context) => Context = context;

        public Task<Subscription[]> GetAll(string userId)
        {
            return Context.Subscriptions.Where(x => x.UserId == userId)
                                        .ToArrayAsync();
        }

        public async Task<Subscription> AddSubscription(Subscription subscription)
        {
            await Context.Subscriptions.AddAsync(subscription);
            await Context.SaveChangesAsync();

            return subscription;
        }

        public async Task UpdateSubscription(Subscription subscription)
        {
            Context.Subscriptions.Update(subscription);
            await Context.SaveChangesAsync();
        }

        public async Task UpdateSubscriptions(Subscription[] subscriptions)
        {
            Context.Subscriptions.UpdateRange(subscriptions);
            await Context.SaveChangesAsync();
        }

        public async Task<Transaction> AddTransaction(Transaction transaction)
        {
            await Context.Transactions.AddAsync(transaction);
            await Context.SaveChangesAsync();

            return transaction;
        }

        public Task<Subscription[]> GetAllWithTransactionId(string transactionId)
        {
            return Context.Subscriptions.Where(x => x.TransactionId == transactionId).ToArrayAsync();
        }
    }
}
