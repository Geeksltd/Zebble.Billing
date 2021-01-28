namespace Zebble.Billing
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Olive;

    public class SubscriptionRepository : ISubscriptionRepository
    {
        readonly BillingContext _context;

        public SubscriptionRepository(BillingContext context) => _context = context;

        public Task<Subscription> GetByPurchaseToken(string purchaseToken)
        {
            return _context.Subscriptions.SingleOrDefaultAsync(x => x.PurchaseToken == purchaseToken);
        }

        public Task<Subscription> GetMostUpdatedByUserId(string userId)
        {
            return _context.Subscriptions.Where(x => x.UserId == userId)
                                         .Where(x => x.DateSubscribed >= LocalTime.Now)
                                         .Where(x => x.ExpiryDate >= LocalTime.Now)
                                         .Where(x => x.CancellationDate == null || x.CancellationDate < LocalTime.Now)
                                         .OrderBy(x => x.ExpiryDate)
                                         .FirstOrDefaultAsync();
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
