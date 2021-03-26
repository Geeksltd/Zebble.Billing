namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using Olive;

    class AppStoreHookInterceptor
    {
        readonly AppStoreOptions Options;
        readonly ISubscriptionRepository Repository;
        readonly AppStoreConnector StoreConnector;

        public AppStoreHookInterceptor(IOptionsSnapshot<AppStoreOptions> options, ISubscriptionRepository repository, AppStoreConnector storeConnector)
        {
            Options = options.Value;
            Repository = repository;
            StoreConnector = storeConnector;
        }

        public async Task Intercept(AppStoreNotification notification)
        {
            ValidateNotification(notification);

            var subscription = await Repository.GetByPurchaseToken(notification.PurchaseToken);

            if (subscription == null)
            {
                var subscriptionInfo = await StoreConnector.GetSubscriptionInfo(new SubscriptionInfoArgs
                {
                    UserId = null,
                    ProductId = notification.ProductId,
                    ReceiptData = notification.UnifiedReceipt.LatestReceipt
                });

                if (subscriptionInfo == null) throw new Exception("Couldn't find receipt info.");

                subscription = await Repository.AddSubscription(new Subscription
                {
                    Id = Guid.NewGuid().ToString(),
                    ProductId = notification.ProductId,
                    UserId = subscriptionInfo.UserId,
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
            }
            else
            {
                if (notification.Type.IsAnyOf(AppStoreNotificationType.InitialBuy, AppStoreNotificationType.InteractivelyRenewed))
                {
                    subscription.SubscriptionDate = notification.PurchaseDate;
                    subscription.ExpirationDate = notification.ExpirationDate;
                    subscription.CancellationDate = notification.CancellationDate;
                    subscription.AutoRenews = notification.AutoRenewStatus;
                }
                else if (notification.Type.IsAnyOf(AppStoreNotificationType.CanceledOrUpgraded, AppStoreNotificationType.Refunded, AppStoreNotificationType.FamilySharingRevoked))
                    subscription.CancellationDate = notification.CancellationDate;
                else if (notification.Type.IsAnyOf(AppStoreNotificationType.AutoRenewed, AppStoreNotificationType.AutoRecovered))
                    subscription.ExpirationDate = notification.ExpirationDate;
                else if (notification.Type == AppStoreNotificationType.AutoRenewFailed)
                    subscription.ExpirationDate = notification.IsInBillingRetryPeriod == true ? notification.GracePeriodExpirationDate : LocalTime.UtcNow;
                else if (notification.Type == AppStoreNotificationType.RenewalStatusChanged)
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

        void ValidateNotification(AppStoreNotification notification)
        {
            if (notification.SharedSecret != Options.SharedSecret) throw new Exception("SharedSecret doesn't match.");

            if (notification.Environment != Options.Environment) throw new Exception("Environment doesn't match.");
        }
    }
}
