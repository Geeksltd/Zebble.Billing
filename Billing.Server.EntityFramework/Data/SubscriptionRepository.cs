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

        public Task<Subscription> GetByTransactionId(string transactionId)
        {
            return Context.Subscriptions.SingleOrDefaultAsync(x => x.TransactionId == transactionId);
        }

        public Task<Subscription> GetByPurchaseToken(string purchaseToken)
        {
            return Context.Subscriptions.SingleOrDefaultAsync(x => x.PurchaseToken == purchaseToken);
        }

        public Task<Subscription> GetMostUpdatedByUserId(string userId)
        {
            return Context.Subscriptions.Where(x => x.UserId == userId)
                                        .OrderBy(x => x.ExpirationDate)
                                        .LastOrDefaultAsync();
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

        public async Task<Transaction> AddTransaction(Transaction transaction)
        {
            await Context.Transactions.AddAsync(transaction);
            await Context.SaveChangesAsync();

            return transaction;
        }

        public Task<string> GetOriginUserOfTransactionIds(string[] transactionIds)
        {
            return Context.Subscriptions.Where(x => transactionIds.Contains(x.TransactionId))
                                        .Select(x => x.UserId)
                                        .FirstOrDefaultAsync();
        }
    }
}
