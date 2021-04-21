namespace Zebble.Billing
{
    using System.Linq;
    using System.Threading.Tasks;
    using Amazon.DynamoDBv2.DataModel;
    using Amazon.DynamoDBv2.DocumentModel;
    using Olive;

    class SubscriptionRepository : ISubscriptionRepository
    {
        readonly SubscriptionDbContext Context;

        public SubscriptionRepository(SubscriptionDbContext context) => Context = context;

        public async Task<Subscription> GetByTransactionId(string transactionId)
        {
            var condition = new ScanCondition(nameof(Subscription.TransactionId), ScanOperator.Equal, transactionId);
            return await Context.FirstOrDefault<SubscriptionProxy>(condition);
        }

        public async Task<Subscription> GetByPurchaseToken(string purchaseToken)
        {
            var condition = new ScanCondition(nameof(Subscription.PurchaseToken), ScanOperator.Equal, purchaseToken);
            return await Context.FirstOrDefault<SubscriptionProxy>(condition);
        }

        public async Task<Subscription> GetMostUpdatedByUserId(string userId)
        {
            var condition = new ScanCondition(nameof(Subscription.UserId), ScanOperator.Equal, userId);
            return (await Context.Where<SubscriptionProxy>(condition)).OrderBy(x => x.ExpirationDate).LastOrDefault();
        }

        public async Task<Subscription> AddSubscription(Subscription subscription)
        {
            await Context.SaveAsync(new SubscriptionProxy(subscription));

            return subscription;
        }

        public Task UpdateSubscription(Subscription subscription)
        {
            return Context.UpdateAsync(x => x.Id, new SubscriptionProxy(subscription));
        }

        public async Task<Transaction> AddTransaction(Transaction transaction)
        {
            await Context.SaveAsync(new TransactionProxy(transaction));

            return transaction;
        }

        public async Task<string> GetOriginUserOfTransactionIds(string[] transactionIds)
        {
            var conditions = transactionIds.Select(x => new ScanCondition(nameof(Subscription.TransactionId), ScanOperator.Equal, x)).ToArray();
            var subscriptions = (await Context.Where<SubscriptionProxy>(conditions)).OrderBy(x => x.ExpirationDate);

            return subscriptions.Where(x => x.IsStarted())
                                .Where(x => !x.IsCanceled())
                                .Where(x => !x.IsExpired())
                                .Select(x => x.UserId)
                                .FirstOrDefault();
        }
    }
}
