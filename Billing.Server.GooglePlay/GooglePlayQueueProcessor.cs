namespace Zebble.Billing
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Google.Cloud.PubSub.V1;
    using Olive;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    class GooglePlayQueueProcessor(
        ILogger<GooglePlayQueueProcessor> logger,
        GooglePlayNotificationProcessor notificationProcessor,
        IServiceProvider services
    )
    {
        public async Task<int> Process()
        {
            var messageCount = 0;

            while (true)
            {
                var chunkCount = 0;

                await using var scope = services.CreateAsyncScope();

                var subscriber = scope.ServiceProvider.GetRequiredService<SubscriberClient>();

                var startTask = subscriber.StartAsync(async (message, _) =>
                {
                    var notification = message.ToNotification();

                    Interlocked.Increment(ref chunkCount);

                    if (!notification.IsTest) await notificationProcessor.Process(notification);

                    return SubscriberClient.Reply.Ack;
                });

                await Task.Delay(2.Seconds());
                await subscriber.StopAsync(new SubscriberClient.ShutdownOptions
                {
                    Mode = SubscriberClient.ShutdownMode.WaitForProcessing
                }, CancellationToken.None);

                await startTask;

                if (chunkCount == 0) break;
                else messageCount += chunkCount;
            }

            logger.Debug($"{messageCount} queue messages are processed.");

            return messageCount;
        }
    }
}
