namespace Zebble.Billing
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Olive;

    class SubscriptionRepository : ISubscriptionRepository
    {
        readonly BillingDbContext context;

        public SubscriptionRepository(BillingDbContext context) => this.context = context;

        public Task<Subscription> GetByPurchaseToken(string purchaseToken)
        {
            return context.Subscriptions.SingleOrDefaultAsync(x => x.PurchaseToken == purchaseToken);
        }

        public Task<Subscription> GetMostUpdatedByUserId(string userId)
        {
            return context.Subscriptions.Where(x => x.UserId == userId)
                                         .Where(x => x.DateSubscribed <= LocalTime.Now)
                                         .Where(x => x.ExpiryDate >= LocalTime.Now)
                                         .Where(x => x.CancellationDate == null || x.CancellationDate >= LocalTime.Now)
                                         .OrderBy(x => x.ExpiryDate)
                                         .LastOrDefaultAsync();
        }

        public async Task<Subscription> AddSubscription(Subscription subscription)
        {
            await context.Subscriptions.AddAsync(subscription);
            await context.SaveChangesAsync();

            return subscription;
        }

        public async Task UpdateSubscription(Subscription subscription)
        {
            context.Subscriptions.Update(subscription);
            await context.SaveChangesAsync();
        }

        public async Task<Transaction> AddTransaction(Transaction transaction)
        {
            await context.Transactions.AddAsync(transaction);
            await context.SaveChangesAsync();

            return transaction;
        }
    }
}
