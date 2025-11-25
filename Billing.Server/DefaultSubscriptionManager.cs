namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Olive;

    public class DefaultSubscriptionManager : ISubscriptionManager
    {
        readonly ILogger<DefaultSubscriptionManager> Logger;
        readonly ISubscriptionRepository Repository;
        readonly ISubscriptionComparer Comparer;
        readonly IStoreConnectorResolver StoreConnectorResolver;
        readonly ISubscriptionChangeHandler SubscriptionChangeHandler;
        readonly BillingOptions Options;

        public DefaultSubscriptionManager(
            ILogger<DefaultSubscriptionManager> logger,
            ISubscriptionRepository repository,
            ISubscriptionComparer comparer,
            IStoreConnectorResolver storeConnectorResolver,
            ISubscriptionChangeHandler subscriptionChangeHandler,
            IOptionsSnapshot<BillingOptions> options
        )
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            StoreConnectorResolver = storeConnectorResolver ?? throw new ArgumentNullException(nameof(storeConnectorResolver));
            SubscriptionChangeHandler = subscriptionChangeHandler ?? throw new ArgumentNullException(nameof(subscriptionChangeHandler));
            Options = options.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public virtual async Task<PurchaseAttemptResult> PurchaseAttempt(string userId, string platform, string productId, string subscriptionId, string transactionId, string purchaseToken, bool replaceConfirmed)
        {
            var storeConnector = StoreConnectorResolver.Resolve(platform);
            var subscriptionInfo = await storeConnector.GetSubscriptionInfo(new SubscriptionInfoArgs
            {
                ProductId = productId,
                SubscriptionId = subscriptionId,
                PurchaseToken = purchaseToken,
                OriginalTransactionId = transactionId,
            });

            if (subscriptionInfo is null)
            {
                Logger.LogWarning($"No subscription info found for token '{purchaseToken}'.");
                Logger.LogWarning($"Additional params {{ userId: {userId}, platform: {platform}, productId: {productId} }}");
                return PurchaseAttemptResult.Failed(new SubscriptionInfo { ProductId = productId });
            }

            // GooglePlay doesn't return expired purchase details, so we've to ensure we have the transaction id.
            subscriptionInfo.TransactionId = subscriptionInfo.TransactionId.Or(transactionId);

            if (subscriptionInfo.TransactionId.IsEmpty())
                return PurchaseAttemptResult.Failed(subscriptionInfo);

            var (isMismatched, originUserId) = await IsSubscriptionMismatched(userId, subscriptionInfo.TransactionId);
            if (isMismatched)
            {
                Logger.LogWarning($"A mismatched subscription info found for token '{purchaseToken}'.");
                Logger.LogWarning($"Additional params {{ userId: {userId}, platform: {platform}, productId: {productId}, resolving strategy: {Options.UserMismatchResolvingStrategy} }}");

                switch (Options.UserMismatchResolvingStrategy)
                {
                    case UserMismatchResolvingStrategy.Block:
                        return PurchaseAttemptResult.UserMismatched(subscriptionInfo, originUserId);
                    case UserMismatchResolvingStrategy.Replace:
                        if (!replaceConfirmed) return PurchaseAttemptResult.UserMismatched(subscriptionInfo, originUserId, userId);
                        await TransferOwnedSubscription(userId, subscriptionInfo.TransactionId);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"{Options.UserMismatchResolvingStrategy} isn't supported.");
                }
            }

            var subscription = await Repository.GetWithTransactionId(subscriptionInfo.TransactionId);

            if (subscription is null)
                subscription = await Repository.AddSubscription(new Subscription
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    Platform = platform,
                    ProductId = productId,
                    SubscriptionId = subscriptionId,
                    TransactionId = subscriptionInfo.TransactionId,
                    TransactionDate = subscriptionInfo.SubscriptionDate,
                    PurchaseToken = purchaseToken,
                    LastUpdate = LocalTime.UtcNow,
                    SubscriptionDate = subscriptionInfo.SubscriptionDate,
                    ExpirationDate = subscriptionInfo.ExpirationDate,
                    CancellationDate = subscriptionInfo.CancellationDate,
                    AutoRenews = subscriptionInfo.AutoRenews
                });
            else
            {
                subscription.TransactionDate = subscriptionInfo.SubscriptionDate;
                subscription.SubscriptionDate = subscriptionInfo.SubscriptionDate;
                subscription.ExpirationDate = subscriptionInfo.ExpirationDate;
                subscription.CancellationDate = subscriptionInfo.CancellationDate;
                subscription.LastUpdate = LocalTime.UtcNow;
                subscription.AutoRenews = subscriptionInfo.AutoRenews;

                await Repository.UpdateSubscription(subscription);
            }

            await SubscriptionChangeHandler.Handle(subscription);

            return PurchaseAttemptResult.Succeeded(subscriptionInfo, originUserId);
        }

        public virtual async Task<Subscription> GetSubscriptionStatus(string userId)
        {
            var subscriptions = await Repository.GetAll(userId);
            if (subscriptions.None())
            {
                Logger.LogWarning($"Found no subscription record for user with id '{userId}'.");
                return null;
            }

            Logger.LogInformation($"Found {subscriptions.Length} subscription records for user with id '{userId}'.");

            var subscription = subscriptions.GetMostRecent(Comparer);

            return subscription;
        }

        protected virtual async Task<(bool, string)> IsSubscriptionMismatched(string userId, string transactionId)
        {
            if (userId.IsEmpty()) return (false, null);
            if (transactionId.IsEmpty()) return (false, null);

            var subscription = await Repository.GetWithTransactionId(transactionId);
            if (subscription is null) return (false, null);

            var originUserId = subscription.UserId;
            if (originUserId.IsEmpty()) return (false, null);

            if (subscription.UserId != userId)
            {
                Logger.LogWarning($"Transaction #{transactionId} is associated to {originUserId} and can't be used for {userId}.");
                return (true, originUserId);
            }

            return (false, null);
        }

        protected virtual async Task TransferOwnedSubscription(string userId, string transactionId)
        {
            var subscription = await Repository.GetWithTransactionId(transactionId);
            if (subscription is null) return;

            Logger.LogWarning($"Found a subscription for transferring to {userId}.");

            subscription.UserId = userId;
            await Repository.UpdateSubscription(subscription);
        }
    }
}
