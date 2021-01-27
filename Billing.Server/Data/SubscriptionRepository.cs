namespace Zebble.Billing
{
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    public class SubscriptionRepository : ISubscriptionRepository
    {
        readonly BillingContext _context;

        public SubscriptionRepository(BillingContext context) => _context = context;

        public Task<Subscription> GetByPurchaseToken(string purchaseToken)
        {
            return _context.Subscriptions.SingleOrDefaultAsync(x => x.PurchaseToken == purchaseToken);
        }

        public async Task<Subscription> Add(Subscription subscription)
        {
            await _context.Subscriptions.AddAsync(subscription);
            await _context.SaveChangesAsync();

            return subscription;
        }

        public async Task Update(Subscription subscription)
        {
            _context.Subscriptions.Update(subscription);
            await _context.SaveChangesAsync();
        }
    }
}
