namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Olive;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    class GooglePlayNotificationProcessor(
        ILogger<GooglePlayNotificationProcessor> logger,
        IServiceProvider services,
        GooglePlayConnector storeConnector,
        ISubscriptionChangeHandler subscriptionChangeHandler)
    {
        public async Task Process(GooglePlayNotification notification)
        {
            try
            {
                await using var scope = services.CreateAsyncScope();
                var repository = scope.ServiceProvider.GetRequiredService<ISubscriptionRepository>();

                // It's a refund notification
                if (notification.OrderId.HasValue())
                {
                    var matchedSubscription = await repository.GetWithTransactionId(notification.OrderId);
                    if (matchedSubscription is null) return;

                    notification.ProductId = matchedSubscription.ProductId;
                }

                var productType = notification.ProductType
                    ?? (notification.State.HasValue ? GooglePlayProductType.Subscription : null);

                var subscriptionInfo = await storeConnector.GetSubscriptionInfo(notification.ToArgs(), productType);
                if (subscriptionInfo is null) return;

                var subscription = await repository.GetWithTransactionId(subscriptionInfo.TransactionId);
                if (subscription is null && notification.PurchaseToken.HasValue())
                    subscription = await repository.GetWithPurchaseToken(notification.PurchaseToken);

                if (subscription is not null)
                {
                    subscription.ProductId = subscriptionInfo.ProductId;
                    subscription.TransactionDate = subscriptionInfo.SubscriptionDate;
                    subscription.SubscriptionDate = subscriptionInfo.SubscriptionDate;
                    subscription.ExpirationDate = subscriptionInfo.ExpirationDate;
                    subscription.CancellationDate = subscriptionInfo.CancellationDate;
                    subscription.LastUpdate = LocalTime.UtcNow;
                    subscription.AutoRenews = subscriptionInfo.AutoRenews;

                    await repository.UpdateSubscription(subscription);

                    await subscriptionChangeHandler.Handle(subscription);
                }

                await repository.AddTransaction(new Transaction
                {
                    Id = Guid.NewGuid().ToString(),
                    SubscriptionId = subscription?.Id,
                    Platform = "GooglePlay",
                    Date = notification.EventTime ?? LocalTime.UtcNow,
                    Details = notification.OriginalData
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process following notification. {NotificationOriginalData}", notification.OriginalData);
                throw;
            }
        }
    }
}
