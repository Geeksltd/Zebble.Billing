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
        readonly ISubscriptionComparer Comparer;
        readonly GooglePlayConnector StoreConnector;
        readonly ISubscriptionChangeHandler SubscriptionChangeHandler;

        public GooglePlayQueueProcessor(
            ILogger<GooglePlayQueueProcessor> logger,
            IServiceProvider services,
            ISubscriptionComparer comparer,
            GooglePlayConnector storeConnector,
            ISubscriptionChangeHandler subscriptionChangeHandler
        )
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Services = services ?? throw new ArgumentNullException(nameof(services));
            Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
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
                using var scope = Services.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<ISubscriptionRepository>();

                var subscriptionInfo = await StoreConnector.GetSubscriptionInfo(notification.ToArgs());
                if (subscriptionInfo.Status != SubscriptionQueryStatus.Succeeded) return;

                var subscriptions = await repository.GetAllWithTransactionId(subscriptionInfo.TransactionId);
                var subscription = subscriptions.GetMostRecent(Comparer);

                if (subscription is null)
                {
                    if (subscriptionInfo.UserId.IsEmpty())
                        subscriptionInfo.UserId = (await repository.GetAllWithPurchaseToken(notification.PurchaseToken).Except(x => x.UserId == "<NOT_PROVIDED>").FirstOrDefault())?.UserId;

                    subscription = await repository.AddSubscription(new Subscription
                    {
                        Id = Guid.NewGuid().ToString(),
                        ProductId = notification.ProductId,
                        UserId = subscriptionInfo.UserId.Or("<NOT_PROVIDED>"),
                        Platform = "GooglePlay",
                        TransactionId = subscriptionInfo.TransactionId,
                        TransactionDate = notification.EventTime,
                        PurchaseToken = notification.PurchaseToken,
                        SubscriptionDate = subscriptionInfo.SubscriptionDate,
                        ExpirationDate = subscriptionInfo.ExpirationDate,
                        CancellationDate = subscriptionInfo.CancellationDate,
                        LastUpdate = LocalTime.UtcNow,
                        AutoRenews = subscriptionInfo.AutoRenews
                    });
                }
                else
                {
                    subscription.TransactionDate = notification.EventTime;
                    subscription.SubscriptionDate = subscriptionInfo.SubscriptionDate;
                    subscription.ExpirationDate = subscriptionInfo.ExpirationDate;
                    subscription.CancellationDate = subscriptionInfo.CancellationDate;
                    subscription.LastUpdate = LocalTime.UtcNow;
                    subscription.AutoRenews = subscriptionInfo.AutoRenews;

                    await repository.UpdateSubscription(subscription);
                }

                await repository.AddTransaction(new Transaction
                {
                    Id = Guid.NewGuid().ToString(),
                    SubscriptionId = subscription.Id,
                    Platform = "GooglePlay",
                    Date = notification.EventTime ?? LocalTime.UtcNow,
                    Details = notification.OriginalData
                });

                await SubscriptionChangeHandler.Handle(subscription);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Failed to process following notification. {notification.OriginalData}");
            }
        }
    }
}
