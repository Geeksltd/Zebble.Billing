namespace Zebble.Billing
{
    using Olive;
    using System;
    using System.Threading.Tasks;

    class SubscriptionManager : ISubscriptionManager
    {
        readonly ISubscriptionRepository repository;
        readonly IPlatformProvider<ILiveSubscriptionQuery> liveSubscriptionQueryProvider;

        public SubscriptionManager(ISubscriptionRepository repository, IPlatformProvider<ILiveSubscriptionQuery> liveSubscriptionQueryProvider)
        {
            this.repository = repository;
            this.liveSubscriptionQueryProvider = liveSubscriptionQueryProvider;
        }

        public async Task InitiatePurchase(string productId, string userId, string platform, string purchaseToken)
        {
            var subscription = await repository.GetByPurchaseToken(purchaseToken);

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

            await repository.AddSubscription(new Subscription
            {
                SubscriptionId = Guid.NewGuid().ToString(),
                ProductId = productId,
                UserId = userId,
                Platform = platform,
                PurchaseToken = purchaseToken,
                LastUpdated = LocalTime.Now
            });
        }

        public async Task<Subscription> GetSubscriptionStatus(string userId)
        {
            var subscription = await repository.GetMostUpdatedByUserId(userId);

            await TryUpdateSubscription(subscription);

            return subscription;
        }

        async Task TryUpdateSubscription(Subscription subscription)
        {
            if (!liveSubscriptionQueryProvider.IsSupported(subscription.Platform)) return;

            var liveSubscriptionQuery = liveSubscriptionQueryProvider[subscription.Platform];
            var updatedSubscription = await liveSubscriptionQuery.GetUpToDateInfo(subscription.ProductId, subscription.PurchaseToken);

            if (updatedSubscription == null) return;

            subscription.ExpiryDate = updatedSubscription.ExpiryDate;
            subscription.CancellationDate = updatedSubscription.CancellationDate;
            subscription.AutoRenews = updatedSubscription.AutoRenews;

            await repository.UpdateSubscription(subscription);
        }
    }
}
