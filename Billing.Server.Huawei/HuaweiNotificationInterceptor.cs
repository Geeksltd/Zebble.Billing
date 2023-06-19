namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Olive;

    class HuaweiNotificationInterceptor
    {
        readonly ILogger<HuaweiNotificationInterceptor> Logger;
        readonly HuaweiOptions Options;
        readonly ISubscriptionRepository Repository;
        readonly ISubscriptionComparer Comparer;
        readonly HuaweiConnector StoreConnector;
        readonly ISubscriptionChangeHandler SubscriptionChangeHandler;

        public HuaweiNotificationInterceptor(
            ILogger<HuaweiNotificationInterceptor> logger,
            IOptionsSnapshot<HuaweiOptions> options,
            ISubscriptionRepository repository,
            ISubscriptionComparer comparer,
            HuaweiConnector storeConnector,
            ISubscriptionChangeHandler subscriptionChangeHandler
        )
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Options = options.Value ?? throw new ArgumentNullException(nameof(options));
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            StoreConnector = storeConnector ?? throw new ArgumentNullException(nameof(storeConnector));
            SubscriptionChangeHandler = subscriptionChangeHandler ?? throw new ArgumentNullException(nameof(subscriptionChangeHandler));
        }

        public async Task Intercept(HuaweiNotification notification)
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
                        SubscriptionId = notification.SubscriptionId,
                        UserId = subscriptionInfo.UserId.Or("<NOT_PROVIDED>"),
                        Platform = "Huawei",
                        TransactionId = subscriptionInfo.TransactionId,
                        PurchaseToken = notification.PurchaseToken,
                        TransactionDate = notification.PurchaseTime,
                        SubscriptionDate = subscriptionInfo.SubscriptionDate,
                        ExpirationDate = subscriptionInfo.ExpirationDate,
                        CancellationDate = subscriptionInfo.CancellationDate,
                        LastUpdate = LocalTime.UtcNow,
                        AutoRenews = subscriptionInfo.AutoRenews
                    });
                else
                {
                    subscription.TransactionDate = notification.PurchaseTime;
                    subscription.SubscriptionDate = notification.PurchaseTime;
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
                    Platform = "Huawei",
                    Date = LocalTime.UtcNow,
                    Details = notification.OriginalData
                });

                await SubscriptionChangeHandler.Handle(subscription);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Failed to intercept the following notification. {notification.OriginalData}");
                throw;
            }
        }

        void ValidateNotification(HuaweiNotification notification)
        {
            if (Options.AllowEnvironmentMixing) return;
            if (notification.Environment != Options.Environment) throw new Exception("Environment doesn't match.");
        }
    }
}
