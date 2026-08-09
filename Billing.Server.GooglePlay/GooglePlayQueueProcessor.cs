namespace Zebble.Billing
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;
    using Google.Api.Gax;
    using Google.Api.Gax.Grpc;
    using Google.Cloud.PubSub.V1;
    using Grpc.Core;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    class GooglePlayQueueProcessor(
        ILogger<GooglePlayQueueProcessor> logger,
        IOptions<GooglePlayOptions> options,
        SubscriberServiceApiClient subscriber,
        IServiceScopeFactory scopeFactory)
    {
        public async Task<int> Process(CancellationToken cancellationToken = default)
        {
            var opts = options.Value;
            var subscriptionName = new SubscriptionName(opts.PubSub.ProjectId, opts.PubSub.SubscriptionId);
            var batchSize = opts.QueuePullBatchSize > 0 ? opts.QueuePullBatchSize : 100;
            var maxMessages = opts.QueueMaxMessagesPerRun > 0 ? opts.QueueMaxMessagesPerRun : 500;
            var parallelism = opts.QueueMaxDegreeOfParallelism > 0 ? opts.QueueMaxDegreeOfParallelism : 8;
            var pullTimeout = TimeSpan.FromSeconds(opts.QueuePullTimeoutSeconds > 0 ? opts.QueuePullTimeoutSeconds : 5);

            var processedCount = 0;
            var pulledCount = 0;

            while (pulledCount < maxMessages && !cancellationToken.IsCancellationRequested)
            {
                var pullCount = Math.Min(batchSize, maxMessages - pulledCount);
                var callSettings = CallSettings
                    .FromCancellationToken(cancellationToken)
                    .WithExpiration(Expiration.FromTimeout(pullTimeout));

                PullResponse response;
                try
                {
                    response = await subscriber.PullAsync(subscriptionName, pullCount, callSettings);
                }
                catch (RpcException ex) when (ex.StatusCode is StatusCode.DeadlineExceeded or StatusCode.Cancelled)
                {
                    break;
                }

                if (response.ReceivedMessages.Count == 0)
                    break;

                pulledCount += response.ReceivedMessages.Count;
                var ackIds = new ConcurrentBag<string>();

                await Parallel.ForEachAsync(
                    response.ReceivedMessages,
                    new ParallelOptions
                    {
                        MaxDegreeOfParallelism = parallelism,
                        CancellationToken = cancellationToken
                    },
                    async (received, ct) =>
                    {
                        try
                        {
                            var notification = received.Message.ToNotification();

                            if (!notification.IsTest)
                            {
                                await using var scope = scopeFactory.CreateAsyncScope();
                                var processor = scope.ServiceProvider.GetRequiredService<GooglePlayNotificationProcessor>();
                                await processor.Process(notification);
                            }

                            ackIds.Add(received.AckId);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Failed to process Pub/Sub message {MessageId}. Message will be redelivered.", received.Message.MessageId);
                        }
                    });

                if (!ackIds.IsEmpty)
                {
                    await subscriber.AcknowledgeAsync(subscriptionName, ackIds, cancellationToken);
                    processedCount += ackIds.Count;
                }

                var failed = response.ReceivedMessages.Count - ackIds.Count;
                if (failed > 0)
                    logger.LogWarning("{FailedCount} of {BatchCount} Google Play queue messages failed and were left unacked.", failed, response.ReceivedMessages.Count);
            }

            logger.LogDebug("{ProcessedCount} queue messages are processed ({PulledCount} pulled).", processedCount, pulledCount);

            return processedCount;
        }
    }
}
