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

        public virtual async Task<PurchaseAttemptResult> PurchaseAttempt(string userId, string platform, string productId, string transactionId, string purchaseToken, bool replaceConfirmed)
        {
            var storeConnector = StoreConnectorResolver.Resolve(platform);
            var subscriptionInfo = await storeConnector.GetSubscriptionInfo(new SubscriptionInfoArgs
            {
                UserId = userId,
                ProductId = productId,
                PurchaseToken = purchaseToken
            });

            if (subscriptionInfo.Status != SubscriptionQueryStatus.Succeeded)
            {
                Logger.LogWarning($"No subscription info found for token '{purchaseToken}'.");
                Logger.LogWarning($"Additional params {{ userId: {userId}, platform: {platform}, productId: {productId} }}");
                return PurchaseAttemptResult.Failed;
            }

            // GooglePlay doesn't return expired purchase details, so we've to ensure we have the transaction id.
            subscriptionInfo.TransactionId = subscriptionInfo.TransactionId.Or(transactionId);

            var (isMismatched, originUserId) = await IsSubscriptionMismatched(userId, productId, subscriptionInfo.TransactionId);
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
                        await CancelAllMatchingSubscriptions(userId, productId, subscriptionInfo.TransactionId);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"{Options.UserMismatchResolvingStrategy} isn't supported.");
                }
            }

            var subscriptions = await Repository.GetAllWithTransactionId(subscriptionInfo.TransactionId);
            var subscription = subscriptions.Where(x => x.UserId == userId)
                                            .Where(x => x.ProductId == productId)
                                            .GetMostRecent(Comparer);

            if (subscription is null)
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

            var subscription = subscriptions.GetMostRecent(Comparer);
            //if (subscription?.IsExpired() == true)
            //    await TryToUpdateSubscription(subscription);

            return subscription;
        }

        protected virtual async Task<(bool, string)> IsSubscriptionMismatched(string userId, string productId, string transactionId)
        {
            if (userId.IsEmpty()) return (false, null);
            if (transactionId.IsEmpty()) return (false, null);

            var subscriptions = await GetMatchingSubscriptionsNotOwnedBy(transactionId, productId, userId);
            var originUserId = subscriptions.Select(x => x.UserId)
                                            .FirstOrDefault(x => x.HasValue());

            if (originUserId.IsEmpty()) return (false, null);

            Logger.LogWarning($"Transaction #{transactionId} is associated to {originUserId} and can't be used for {userId}.");

            return (true, originUserId);
        }

        protected virtual async Task CancelAllMatchingSubscriptions(string userId, string productId, string transactionId)
        {
            var subscriptions = await GetMatchingSubscriptionsNotOwnedBy(transactionId, productId, userId);
            Logger.LogWarning($"Found {subscriptions.Length} subscriptions for cancellation.");

            subscriptions.Do(x =>
            {
                x.CancellationDate = LocalTime.UtcNow;
                // We do not ignore the cancelled records, so we need to also update the expiration date.
                x.ExpirationDate = LocalTime.UtcNow;
            });
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

        async Task<Subscription[]> GetMatchingSubscriptionsNotOwnedBy(string transactionId, string productId, string userId)
        {
            var subscriptions = await Repository.GetAllWithTransactionId(transactionId);
            return subscriptions.Where(x => x.ProductId == productId)
                                .Except(x => x.UserId == userId)
                                .Except(x => x.IsExpired())
                                .Except(x => x.IsCanceled())
                                .ToArray();
        }
    }
}
