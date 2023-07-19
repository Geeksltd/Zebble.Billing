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
        readonly HuaweiConnector StoreConnector;
        readonly ISubscriptionChangeHandler SubscriptionChangeHandler;

        public HuaweiNotificationInterceptor(
            ILogger<HuaweiNotificationInterceptor> logger,
            IOptionsSnapshot<HuaweiOptions> options,
            ISubscriptionRepository repository,
            HuaweiConnector storeConnector,
            ISubscriptionChangeHandler subscriptionChangeHandler
        )
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Options = options.Value ?? throw new ArgumentNullException(nameof(options));
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            StoreConnector = storeConnector ?? throw new ArgumentNullException(nameof(storeConnector));
            SubscriptionChangeHandler = subscriptionChangeHandler ?? throw new ArgumentNullException(nameof(subscriptionChangeHandler));
        }

        public async Task Intercept(HuaweiNotification notification)
        {
            try
            {
                ValidateNotification(notification);

                var subscriptionInfo = await StoreConnector.GetSubscriptionInfo(notification.ToArgs());
                if (subscriptionInfo is null) return;

                var subscription = await Repository.GetWithTransactionId(subscriptionInfo.TransactionId);

                if (subscription is not null)
                {
                    subscription.TransactionDate = subscriptionInfo.SubscriptionDate;
                    subscription.SubscriptionDate = subscriptionInfo.SubscriptionDate;
                    subscription.ExpirationDate = subscriptionInfo.ExpirationDate;
                    subscription.CancellationDate = subscriptionInfo.CancellationDate;
                    subscription.LastUpdate = LocalTime.UtcNow;
                    subscription.AutoRenews = subscriptionInfo.AutoRenews;

                    await Repository.UpdateSubscription(subscription);

                    await SubscriptionChangeHandler.Handle(subscription);
                }

                await Repository.AddTransaction(new Transaction
                {
                    Id = Guid.NewGuid().ToString(),
                    SubscriptionId = subscription?.Id,
                    Platform = "Huawei",
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

        void ValidateNotification(HuaweiNotification notification)
        {
            if (Options.AllowEnvironmentMixing) return;
            if (notification.Environment != Options.Environment) throw new Exception("Environment doesn't match.");
        }
    }
}
