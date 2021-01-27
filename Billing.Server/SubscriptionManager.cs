namespace Zebble.Billing
{
    using Olive;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class SubscriptionManager : ISubscriptionManager
    {
        private readonly ISubscriptionRepository _subscriptionRepository;

        public SubscriptionManager(ISubscriptionRepository subscriptionRepository)
        {
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task InitiatePurchase(string productId, string userId, SubscriptionPlatform platform, string purchaseToken)
        {
            var subscription = await _subscriptionRepository.GetByPurchaseToken(purchaseToken);

            if (subscription != null && subscription.UserId != userId)
                throw new Exception("Provided purchase token is associated with another user!");

            await _subscriptionRepository.Add(new Subscription
            {
                SubscriptionId = Guid.NewGuid(),
                ProductId = productId,
                UserId = userId,
                Platform = platform,
                PurchaseToken = purchaseToken,
                LastUpdated = LocalTime.Now
            });
        }

        public Task<Subscription[]> RefreshSubscriptions(string userId, string[] purchaseTokens)
        {
            throw new System.NotImplementedException();
        }
    }
}
