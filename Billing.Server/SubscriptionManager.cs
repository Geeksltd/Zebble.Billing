namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Olive;

    public class SubscriptionManager
    {
        readonly ISubscriptionRepository Repository;
        readonly StoreConnectorResolver StoreConnectorResolver;

        internal SubscriptionManager(ISubscriptionRepository repository, StoreConnectorResolver storeConnectorResolver)
        {
            Repository = repository;
            StoreConnectorResolver = storeConnectorResolver;
        }

        public async Task PurchaseAttempt(string productId, string userId, string platform, string purchaseToken)
        {
            var subscription = await Repository.GetByPurchaseToken(purchaseToken);

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

            await Repository.AddSubscription(new Subscription
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
            var subscription = await Repository.GetMostUpdatedByUserId(userId);

            if (subscription?.RequiresStoreUpdate() == true)
                await TryToUpdateSubscription(subscription);

            return subscription;
        }

        async Task TryToUpdateSubscription(Subscription subscription)
        {
            var storeConnector = StoreConnectorResolver.Resolve(subscription.Platform);
            var updatedSubscription = await storeConnector.GetUpToDateInfo(subscription.ProductId, subscription.PurchaseToken);

            if (updatedSubscription == null) return;

            subscription.ExpiryDate = updatedSubscription.ExpiryDate;
            subscription.CancellationDate = updatedSubscription.CancellationDate;
            subscription.AutoRenews = updatedSubscription.AutoRenews;

            await Repository.UpdateSubscription(subscription);
        }
    }
}
