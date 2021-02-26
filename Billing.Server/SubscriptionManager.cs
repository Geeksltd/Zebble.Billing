namespace Zebble.Billing
{
    using System;
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

        public async Task<bool> VerifyPurchase(string userId, string platform, string productId, string transactionId, string receiptData)
        {
            var storeConnector = StoreConnectorResolver.Resolve(platform);
            var isValid = await storeConnector.VerifyPurchase(new VerifyPurchaseArgs
            {
                ReceiptData = receiptData
            });

            if (!isValid) return false;

            var subscription = await Repository.GetByTransactionId(transactionId);

            if (subscription != null)
            {
                if (subscription.UserId != userId)
                    throw new Exception("Provided purchase token is associated with another user!");

                if (subscription.Platform != platform)
                    throw new Exception("Provided purchase token is associated with another platform!");

                if (subscription.ProductId != productId)
                    throw new Exception("Provided purchase token is associated with another product!");

                return true;
            }

            await Repository.AddSubscription(new Subscription
            {
                SubscriptionId = Guid.NewGuid().ToString(),
                UserId = userId,
                Platform = platform,
                ProductId = productId,
                TransactionId = transactionId,
                ReceiptData = receiptData,
                LastUpdate = LocalTime.UtcNow
            });

            return true;
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
            var subscription = await Repository.GetMostUpdatedByUserId(userId);

            if (subscription?.RequiresStoreUpdate() == true)
                await TryToUpdateSubscription(subscription);

            return subscription;
        }

        async Task TryToUpdateSubscription(Subscription subscription)
        {
            var storeConnector = StoreConnectorResolver.Resolve(subscription.Platform);
            var updatedSubscription = await storeConnector.GetSubscriptionInfo(subscription.ToArgs());

            if (updatedSubscription == null) return;

            subscription.SubscriptionDate = updatedSubscription.SubscriptionDate;
            subscription.ExpirationDate = updatedSubscription.ExpirationDate;
            subscription.CancellationDate = updatedSubscription.CancellationDate;
            subscription.AutoRenews = updatedSubscription.AutoRenews;

            await Repository.UpdateSubscription(subscription);
        }
    }
}
