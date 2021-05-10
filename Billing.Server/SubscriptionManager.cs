namespace Zebble.Billing
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Olive;

    class SubscriptionManager
    {
        readonly ISubscriptionRepository Repository;
        readonly IStoreConnectorResolver StoreConnectorResolver;

        public SubscriptionManager(ISubscriptionRepository repository, IStoreConnectorResolver storeConnectorResolver)
        {
            Repository = repository;
            StoreConnectorResolver = storeConnectorResolver;
        }

        public async Task<VerifyPurchaseResult> VerifyPurchase(string userId, string platform, string productId, string transactionId, string receiptData)
        {
            var storeConnector = StoreConnectorResolver.Resolve(platform);
            var status = await storeConnector.VerifyPurchase(new VerifyPurchaseArgs
            {
                UserId = userId,
                ReceiptData = receiptData
            });

            if (status != PurchaseVerificationStatus.Verified) return VerifyPurchaseResult.From(status);

            var subscription = await Repository.GetByTransactionId(transactionId);

            if (subscription is not null)
            {
                if (subscription.UserId != userId)
                    throw new Exception("Provided purchase token is associated with another user!");

                if (subscription.Platform != platform)
                    throw new Exception("Provided purchase token is associated with another platform!");

                if (subscription.ProductId != productId)
                    throw new Exception("Provided purchase token is associated with another product!");
            }
            else
                await Repository.AddSubscription(new Subscription
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    Platform = platform,
                    ProductId = productId,
                    TransactionId = transactionId,
                    ReceiptData = receiptData,
                    LastUpdate = LocalTime.UtcNow
                });

            return VerifyPurchaseResult.Succeeded();
        }

        public async Task PurchaseAttempt(string userId, string platform, string productId, string transactionId, DateTime transactionDate, string purchaseToken)
        {
            var subscription = await Repository.GetByTransactionId(transactionId);

            if (subscription is null) throw new Exception($"No subscription found for transaction '{transactionId}'.");

            if (subscription.UserId != userId)
                throw new Exception("Provided purchase token is associated with another user!");

            if (subscription.Platform != platform)
                throw new Exception("Provided purchase token is associated with another platform!");

            if (subscription.ProductId != productId)
                throw new Exception("Provided purchase token is associated with another product!");

            subscription.TransactionDate = transactionDate;
            subscription.PurchaseToken = purchaseToken;
            subscription.LastUpdate = LocalTime.UtcNow;

            var storeConnector = StoreConnectorResolver.Resolve(platform);
            var subscriptionInfo = await storeConnector.GetSubscriptionInfo(subscription.ToArgs());

            subscription.SubscriptionDate = subscriptionInfo?.SubscriptionDate;
            subscription.ExpirationDate = subscriptionInfo?.ExpirationDate;
            subscription.CancellationDate = subscriptionInfo?.CancellationDate;
            subscription.AutoRenews = subscriptionInfo?.AutoRenews;

            await Repository.UpdateSubscription(subscription);
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
            var updatedSubscription = await storeConnector.GetSubscriptionInfo(subscription.ToArgs());

            if (updatedSubscription is null) return;

            subscription.SubscriptionDate = updatedSubscription.SubscriptionDate;
            subscription.ExpirationDate = updatedSubscription.ExpirationDate;
            subscription.CancellationDate = updatedSubscription.CancellationDate;
            subscription.AutoRenews = updatedSubscription.AutoRenews;

            await Repository.UpdateSubscription(subscription);
        }
    }
}
