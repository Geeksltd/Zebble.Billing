namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Olive;

    class AppStoreHookInterceptor
    {
        readonly ILogger<AppStoreHookInterceptor> Logger;
        readonly AppStoreOptions Options;
        readonly ISubscriptionRepository Repository;
        readonly ISubscriptionComparer Comparer;
        readonly AppStoreConnector StoreConnector;

        public AppStoreHookInterceptor(
            ILogger<AppStoreHookInterceptor> logger,
            IOptionsSnapshot<AppStoreOptions> options,
            ISubscriptionRepository repository,
            ISubscriptionComparer comparer,
            AppStoreConnector storeConnector
        )
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Options = options.Value ?? throw new ArgumentNullException(nameof(options));
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            StoreConnector = storeConnector ?? throw new ArgumentNullException(nameof(storeConnector));
        }

        public async Task Intercept(AppStoreNotification notification)
        {
            try
            {
                ValidateNotification(notification);

                var subscriptionInfo = await StoreConnector.GetSubscriptionInfo(notification.ToArgs());
                if (subscriptionInfo.Status != SubscriptionQueryStatus.Succeeded) return;

                var subscriptions = await Repository.GetAllWithTransactionId(subscriptionInfo.TransactionId);
                var subscription = subscriptions.GetMostRecent(Comparer);

                if (subscription is null)
                    subscription = await Repository.AddSubscription(new Subscription
                    {
                        Id = Guid.NewGuid().ToString(),
                        ProductId = notification.ProductId,
                        UserId = subscriptionInfo.UserId.Or("<NOT_PROVIDED>"),
                        Platform = "AppStore",
                        TransactionId = subscriptionInfo.TransactionId,
                        PurchaseToken = notification.PurchaseToken,
                        TransactionDate = notification.PurchaseDate,
                        SubscriptionDate = subscriptionInfo.SubscriptionDate,
                        ExpirationDate = subscriptionInfo.ExpirationDate,
                        CancellationDate = subscriptionInfo.CancellationDate,
                        LastUpdate = LocalTime.UtcNow,
                        AutoRenews = subscriptionInfo.AutoRenews
                    });
                else
                {
                    subscription.TransactionDate = notification.PurchaseDate;
                    subscription.SubscriptionDate = notification.PurchaseDate;
                    subscription.ExpirationDate = notification.ExpirationDate;
                    subscription.CancellationDate = notification.CancellationDate;
                    subscription.LastUpdate = LocalTime.UtcNow;
                    subscription.AutoRenews = notification.AutoRenewStatus;

                    await Repository.UpdateSubscription(subscription);
                }

                await Repository.AddTransaction(new Transaction
                {
                    Id = Guid.NewGuid().ToString(),
                    SubscriptionId = subscription.Id,
                    Platform = "AppStore",
                    Date = LocalTime.UtcNow,
                    Details = notification.OriginalData
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Failed to intercept the following notification. {notification.OriginalData}");
                throw;
            }
        }

        void ValidateNotification(AppStoreNotification notification)
        {
            if (notification.SharedSecret != Options.SharedSecret) throw new Exception("SharedSecret doesn't match.");

            if (notification.Environment != Options.Environment) throw new Exception("Environment doesn't match.");
        }
    }
}
