namespace Zebble.Billing
{
    using System;
    using System.Linq;
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
        readonly BillingOptions Options;

        public DefaultSubscriptionManager(
            ILogger<DefaultSubscriptionManager> logger,
            ISubscriptionRepository repository,
            ISubscriptionComparer comparer,
            IStoreConnectorResolver storeConnectorResolver,
            IOptionsSnapshot<BillingOptions> options
        )
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            StoreConnectorResolver = storeConnectorResolver ?? throw new ArgumentNullException(nameof(storeConnectorResolver));
            Options = options.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public virtual async Task<PurchaseAttemptResult> PurchaseAttempt(string userId, string platform, string productId, string purchaseToken, bool replaceConfirmed)
        {
            var subscription = await Repository.GetByPurchaseToken(purchaseToken);
            if (subscription is not null)
            {
                Logger.LogWarning($"An existing subscription found for token '{purchaseToken}'.");
                Logger.LogWarning($"Additional params {{ userId: {userId}, platform: {platform}, productId: {productId} }}");
                Logger.LogWarning($"Existing params {{ userId: {subscription.UserId}, platform: {subscription.Platform}, productId: {subscription.ProductId} }}");
                throw new Exception($"An existing subscription found for token '{purchaseToken}'.");
            }

            var storeConnector = StoreConnectorResolver.Resolve(platform);
            var subscriptionInfo = await storeConnector.GetSubscriptionInfo(new SubscriptionInfoArgs
            {
                UserId = userId,
                ProductId = productId,
                PurchaseToken = purchaseToken
            });

            if (subscriptionInfo.Status == SubscriptionQueryStatus.NotFound)
            {
                Logger.LogWarning($"No subscription info found for token '{purchaseToken}'.");
                Logger.LogWarning($"Additional params {{ userId: {userId}, platform: {platform}, productId: {productId} }}");
                return PurchaseAttemptResult.Failed;
            }

            var (isMismatched, originUserId) = await IsSubscriptionMismatched(userId, subscriptionInfo);
            if (isMismatched)
            {
                Logger.LogWarning($"A mismatched subscription info found for token '{purchaseToken}'.");
                Logger.LogWarning($"Additional params {{ userId: {userId}, platform: {platform}, productId: {productId}, resolving strategy: {Options.UserMismatchResolvingStrategy} }}");

                switch (Options.UserMismatchResolvingStrategy)
                {
                    case UserMismatchResolvingStrategy.Block:
                        return PurchaseAttemptResult.UserMismatched(originUserId);
                    case UserMismatchResolvingStrategy.Replace:
                        if (!replaceConfirmed) return PurchaseAttemptResult.UserMismatched(originUserId, userId);
                        await CancelAllMatchingSubscriptions(userId, subscriptionInfo.TransactionId);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"{Options.UserMismatchResolvingStrategy} isn't supported.");
                }
            }

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

            return PurchaseAttemptResult.Succeeded(originUserId);
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

            var subscription = subscriptions.OrderBy(x => x, Comparer).LastOrDefault();
            if (subscription?.RequiresStoreUpdate() == true)
                await TryToUpdateSubscription(subscription);

            return subscription;
        }

        protected virtual async Task<(bool, string)> IsSubscriptionMismatched(string userId, SubscriptionInfo subscriptionInfo)
        {
            if (userId.IsEmpty()) return (false, null);

            if (subscriptionInfo.TransactionId.IsEmpty()) return (false, null);

            var originUserId = await Repository.GetOriginUserOfTransactionId(subscriptionInfo.TransactionId);
            if (originUserId.IsEmpty()) return (false, null);
            if (originUserId.Equals(userId, caseSensitive: false)) return (false, null);

            Logger.LogWarning($"Transaction #{subscriptionInfo.TransactionId} is associated to {originUserId} and can't be used for {userId}.");

            return (true, originUserId);
        }

        protected virtual async Task CancelAllMatchingSubscriptions(string userId, string transactionId)
        {
            var subscriptions = await Repository.GetAllWithTransactionIdNotOwnedBy(userId, transactionId);
            Logger.LogWarning($"Found {subscriptions.Length} subscriptions for cancellation.");

            subscriptions.Do(x => x.CancellationDate = LocalTime.UtcNow);
            await Repository.UpdateSubscriptions(subscriptions);
            Logger.LogWarning($"{subscriptions.Length} subscriptions have been cancelled.");
            Logger.LogWarning($"Affected user ids: {subscriptions.Select(x => x.UserId).Distinct().ToString(", ")}");
        }

        protected virtual async Task TryToUpdateSubscription(Subscription subscription)
        {
            var storeConnector = StoreConnectorResolver.Resolve(subscription.Platform);
            var subscriptionInfo = await storeConnector.GetSubscriptionInfo(subscription.ToArgs());

            if (subscriptionInfo.Status != SubscriptionQueryStatus.Succeeded)
            {
                Logger.LogWarning($"Subscription with id '{subscription.Id}' needed to be updated, but we were not able to pull fresh data from the store.");
                Logger.LogWarning($"Additional params {{ userId: {subscription.UserId}, platform: {subscription.Platform}, productId: {subscription.ProductId} }}");
                return;
            }

            subscription.SubscriptionDate = subscriptionInfo.SubscriptionDate;
            subscription.ExpirationDate = subscriptionInfo.ExpirationDate;
            subscription.CancellationDate = subscriptionInfo.CancellationDate;
            subscription.AutoRenews = subscriptionInfo.AutoRenews;

            await Repository.UpdateSubscription(subscription);
        }
    }
}
