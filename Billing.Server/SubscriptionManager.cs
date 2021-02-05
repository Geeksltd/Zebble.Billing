namespace Zebble.Billing
{
    using Olive;
    using System;
    using System.Threading.Tasks;

    class SubscriptionManager : ISubscriptionManager
    {
        readonly ISubscriptionRepository _subscriptionRepository;
        readonly IPlatformProvider<ILiveSubscriptionQuery> _liveSubscriptionQueryProvider;

        public SubscriptionManager(ISubscriptionRepository subscriptionRepository, IPlatformProvider<ILiveSubscriptionQuery> liveSubscriptionQueryProvider)
        {
            _subscriptionRepository = subscriptionRepository;
            _liveSubscriptionQueryProvider = liveSubscriptionQueryProvider;
        }

        public async Task InitiatePurchase(string productId, string userId, SubscriptionPlatform platform, string purchaseToken)
        {
            var subscription = await _subscriptionRepository.GetByPurchaseToken(purchaseToken);

            if (subscription != null)
            {
                if (subscription.ProductId != productId)
                    throw new Exception("Provided purchase token is associated with another product!");

                if (subscription.UserId != userId)
                    throw new Exception("Provided purchase token is associated with another user!");

                if (subscription.Platform != platform)
                    throw new Exception("Provided purchase token is associated with another platform!");

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

        public async Task<Subscription> GetSubscriptionStatus(string userId)
        {
            var subscription = await _subscriptionRepository.GetMostUpdatedByUserId(userId);

            await TryUpdateSubscription(subscription);

            return subscription;
        }

        async Task TryUpdateSubscription(Subscription subscription)
        {
            if (!_liveSubscriptionQueryProvider.IsSupported(subscription.Platform)) return;

            var liveSubscriptionQuery = _liveSubscriptionQueryProvider[subscription.Platform];
            var updatedSubscription = await liveSubscriptionQuery.GetUpToDateInfo(subscription.ProductId, subscription.PurchaseToken);

            if (updatedSubscription == null) return;

            subscription.ExpiryDate = updatedSubscription.ExpiryDate;
            subscription.CancellationDate = updatedSubscription.CancellationDate;
            subscription.AutoRenews = updatedSubscription.AutoRenews;

            await _subscriptionRepository.Update(subscription);
        }
    }
}
