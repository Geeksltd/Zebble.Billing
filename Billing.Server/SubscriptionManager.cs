namespace Zebble.Billing
{
    using Olive;
    using System;
    using System.Threading.Tasks;

    public class SubscriptionManager : ISubscriptionManager
    {
        readonly ISubscriptionRepository _subscriptionRepository;

        public SubscriptionManager(ISubscriptionRepository subscriptionRepository)
        {
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task InitiatePurchase(string productId, string userId, SubscriptionPlatform platform, string purchaseToken)
        {
            var subscription = await _subscriptionRepository.GetByPurchaseToken(purchaseToken);

            if (subscription != null)
            {
                if (subscription.UserId != userId)
                    throw new Exception("Provided purchase token is associated with another user!");

                return;
            }

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

        public Task<Subscription> GetSubscriptionInfo(string userId)
        {
            return _subscriptionRepository.GetMostUpdatedByUserId(userId);
        }
    }
}
