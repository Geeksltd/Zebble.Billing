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

        public async Task Intercept(string body)
        {
            var notification = body.ToNotification();

            await ProcessNotification(notification);
        }

        async Task ProcessNotification(AppStoreNotification notification)
        {
            ValidateNotification(notification);

            var subscription = await Repository.GetByPurchaseToken(notification.PurchaseToken);

            if (subscription == null)
            {
                subscription = await StoreConnector.GetUpToDateInfo(notification.ProductId, notification.PurchaseToken);

                if (subscription == null)
                    throw new Exception("Couldn't find subscription info.");

                subscription = await Repository.AddSubscription(subscription);
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
                    subscription.ExpirationDate = notification.IsInBillingRetryPeriod == true ? notification.GracePeriodExpirationDate : LocalTime.Now;
                else if (notification.Type == AppStoreNotificationType.RenewalStatusChanged)
                    subscription.AutoRenews = notification.AutoRenewStatus;

                await Repository.UpdateSubscription(subscription);
            }

            await Repository.AddTransaction(new Transaction
            {
                TransactionId = Guid.NewGuid().ToString(),
                SubscriptionId = subscription.SubscriptionId,
                Platform = "AppStore",
                Date = LocalTime.Now,
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
