namespace Zebble.Billing
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Google.Cloud.PubSub.V1;
    using Olive;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    class GooglePlayQueueProcessor
    {
        readonly ILogger<GooglePlayQueueProcessor> Logger;
        readonly IServiceProvider Services;
        readonly GooglePlayConnector StoreConnector;
        readonly ISubscriptionChangeHandler SubscriptionChangeHandler;

        public GooglePlayQueueProcessor(
            ILogger<GooglePlayQueueProcessor> logger,
            IServiceProvider services,
            GooglePlayConnector storeConnector,
            ISubscriptionChangeHandler subscriptionChangeHandler
        )
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Services = services ?? throw new ArgumentNullException(nameof(services));
            StoreConnector = storeConnector ?? throw new ArgumentNullException(nameof(storeConnector));
            SubscriptionChangeHandler = subscriptionChangeHandler ?? throw new ArgumentNullException(nameof(subscriptionChangeHandler));
        }

        public async Task<int> Process()
        {
            var messageCount = 0;

            while (true)
            {
                var chunkCount = 0;

                var subscriber = Services.GetService<SubscriberClient>();

                var startTask = subscriber.StartAsync(async (message, _) =>
                {
                    var notification = message.ToNotification();

                    Interlocked.Increment(ref chunkCount);

                    if (!notification.IsTest) await ProccessNotification(notification);

                    return SubscriberClient.Reply.Ack;
                });

                await Task.Delay(2.Seconds());
                await subscriber.StopAsync(CancellationToken.None);

                await startTask;

                if (chunkCount == 0) break;
                else messageCount += chunkCount;
            }

            Logger.Debug($"{messageCount} queue messages are processed.");

            return messageCount;
        }

        async Task ProccessNotification(GooglePlayNotification notification)
        {
            try
            {
                await using var scope = Services.CreateAsyncScope();
                var repository = scope.ServiceProvider.GetRequiredService<ISubscriptionRepository>();

                // It's a refund notification
                if (notification.OrderId.HasValue())
                {
                    var matchedSubscription = await repository.GetWithTransactionId(notification.OrderId);
                    if (matchedSubscription is null) return;

                    notification.ProductId = matchedSubscription.ProductId;
                }

                var subscriptionInfo = await StoreConnector.GetSubscriptionInfo(notification.ToArgs());
                if (subscriptionInfo is null) return;

                var subscription = await repository.GetWithTransactionId(subscriptionInfo.TransactionId);

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

                    await SubscriptionChangeHandler.Handle(subscription);
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
                Logger.LogError(ex, $"Failed to process following notification. {notification.OriginalData}");
            }
        }
    }
}
