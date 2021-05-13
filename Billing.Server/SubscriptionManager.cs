namespace Zebble.Billing
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Olive;

    public class SubscriptionManager
    {
        readonly ISubscriptionRepository Repository;
        readonly IStoreConnectorResolver StoreConnectorResolver;

        public SubscriptionManager(ISubscriptionRepository repository, IStoreConnectorResolver storeConnectorResolver)
        {
            Repository = repository;
            StoreConnectorResolver = storeConnectorResolver;
        }

        public async Task<PurchaseAttemptResult> PurchaseAttempt(string userId, string platform, string productId, string purchaseToken)
        {
            var subscription = await Repository.GetByPurchaseToken(purchaseToken);
            if (subscription is not null)
                throw new Exception($"An existing subscription found for token '{purchaseToken}'.");

            var storeConnector = StoreConnectorResolver.Resolve(platform);
            var subscriptionInfo = await storeConnector.GetSubscriptionInfo(new SubscriptionInfoArgs
            {
                UserId = userId,
                ProductId = productId,
                PurchaseToken = purchaseToken
            });

            if (subscriptionInfo.Status == SubscriptionQueryStatus.NotFound) return PurchaseAttemptResult.Failed;
            if (subscriptionInfo.Status == SubscriptionQueryStatus.UserMismatched) return PurchaseAttemptResult.UserMismatched;

            subscription = await Repository.GetByTransactionId(subscriptionInfo.TransactionId);
            if (subscription is not null)
                throw new Exception($"An existing subscription found for transaction '{subscriptionInfo.TransactionId}'.");

            await Repository.AddSubscription(new Subscription
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                Platform = platform,
                ProductId = productId,
                TransactionId = subscriptionInfo.TransactionId,
                TransactionDate = subscriptionInfo.SubscriptionDate,
                PurchaseToken = purchaseToken,
                LastUpdate = LocalTime.UtcNow,
                SubscriptionDate = subscriptionInfo.SubscriptionDate,
                ExpirationDate = subscriptionInfo.ExpirationDate,
                CancellationDate = subscriptionInfo.CancellationDate,
                AutoRenews = subscriptionInfo.AutoRenews
            });

            return PurchaseAttemptResult.Succeeded;
        }

        public async Task<Subscription> GetSubscriptionStatus(string userId)
        {
            var subscriptions = await Repository.GetAll(userId);

            var subscription = subscriptions.OrderBy(x => x.SubscriptionDate).LastOrDefault();

            if (subscription?.IsActive() == false)
                subscription = subscriptions.OrderBy(x => x.ExpirationDate).LastOrDefault();

            if (subscription?.RequiresStoreUpdate() == true)
                await TryToUpdateSubscription(subscription);

            return subscription;
        }

        async Task TryToUpdateSubscription(Subscription subscription)
        {
            var storeConnector = StoreConnectorResolver.Resolve(subscription.Platform);
            var subscriptionInfo = await storeConnector.GetSubscriptionInfo(subscription.ToArgs());

            if (subscriptionInfo.Status != SubscriptionQueryStatus.Succeeded) return;

            subscription.SubscriptionDate = subscriptionInfo.SubscriptionDate;
            subscription.ExpirationDate = subscriptionInfo.ExpirationDate;
            subscription.CancellationDate = subscriptionInfo.CancellationDate;
            subscription.AutoRenews = subscriptionInfo.AutoRenews;

            await Repository.UpdateSubscription(subscription);
        }
    }
}
