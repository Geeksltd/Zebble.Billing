namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using Google.Apis.Auth.OAuth2;
    using Google.Cloud.PubSub.V1;
    using Grpc.Auth;
    using Olive;

    class GooglePlayQueueProcessor : IQueueProcessor
    {
        readonly GooglePubSubOptions options;
        readonly ISubscriptionRepository repository;
        readonly ILiveSubscriptionQuery liveSubscriptionQuery;

        public GooglePlayQueueProcessor(
            IOptionsSnapshot<GooglePubSubOptions> options,
            ISubscriptionRepository repository,
            GooglePlayLiveSubscriptionQuery liveSubscriptionQuery
        )
        {
            this.options = options.Value;
            this.repository = repository;
            this.liveSubscriptionQuery = liveSubscriptionQuery;
        }

        public async Task<int> Process()
        {
            var handled = 0;

            var name = new SubscriptionName(options.ProjectId, options.SubscriptionId);
            var client = await SubscriberClient.CreateAsync(name, GetSettings());

            var startTask = client.StartAsync(async (message, _) =>
            {
                var notification = message.ToNotification();

                if (notification.IsTest == false && !await ProccessNotification(notification))
                    return SubscriberClient.Reply.Nack;

                handled++;

                return SubscriberClient.Reply.Ack;
            });

            await Task.Delay(1.Seconds());
            var stopTask = client.StopAsync(10.Seconds());

            await startTask;
            await stopTask;

            return handled;
        }

        async Task<bool> ProccessNotification(GoogleNotification notification)
        {
            var subscription = await repository.GetByPurchaseToken(notification.PurchaseToken);

            if (subscription == null)
            {
                subscription = await liveSubscriptionQuery.GetUpToDateInfo(notification.ProductId, notification.PurchaseToken);

                if (subscription == null)
                    return false;

                subscription = await repository.AddSubscription(subscription);
            }
            else
            {
                if (notification.State == GoogleNotification.SubscriptionState.Canceled)
                    subscription.CancellationDate = notification.EventTime;
                else if (notification.State == GoogleNotification.SubscriptionState.Expired)
                    subscription.ExpiryDate = notification.EventTime;

                await repository.UpdateSubscription(subscription);
            }

            await repository.AddTransaction(new Transaction
            {
                TransactionId = Guid.NewGuid().ToString(),
                SubscriptionId = subscription.SubscriptionId,
                Platform = "GooglePlay",
                Date = notification.EventTime,
                Details = notification.OriginalData
            });

            return true;
        }

        SubscriberClient.ClientCreationSettings GetSettings()
        {
            var json = new JsonCredentialParameters
            {
                Type = "service_account",
                ProjectId = options.ProjectId,
                PrivateKeyId = options.PrivateKeyId,
                PrivateKey = options.PrivateKey,
                ClientEmail = options.ClientEmail,
                ClientId = options.ClientId
            }.ToJson();

            return new SubscriberClient.ClientCreationSettings(
                credentials: GoogleCredential.FromJson(json).ToChannelCredentials()
            );
        }
    }
}
