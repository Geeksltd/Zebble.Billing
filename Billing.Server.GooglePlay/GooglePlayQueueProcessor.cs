namespace Zebble.Billing
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Text.Json;
    using Microsoft.Extensions.Options;
    using Google.Apis.Auth.OAuth2;
    using Google.Cloud.PubSub.V1;
    using Grpc.Auth;
    using Olive;

    class GooglePlayQueueProcessor
    {
        readonly GooglePlayOptions Options;
        readonly ISubscriptionRepository Repository;
        readonly GooglePlayConnector StoreConnector;

        public GooglePlayQueueProcessor(IOptionsSnapshot<GooglePlayOptions> options, ISubscriptionRepository repository, GooglePlayConnector storeConnector)
        {
            Options = options.Value;
            Repository = repository;
            StoreConnector = storeConnector;
        }

        public async Task<int> Process()
        {
            var name = new SubscriptionName(Options.ProjectId, Options.SubscriptionId);
            var client = await SubscriberClient.CreateAsync(name, GetSettings());
            var messageCount = 0;

            var startTask = client.StartAsync(async (message, _) =>
            {
                var notification = message.ToNotification();

                Interlocked.Increment(ref messageCount);

                if (notification.IsTest == false && !await ProccessNotification(notification))
                    return SubscriberClient.Reply.Nack;

                return SubscriberClient.Reply.Ack;
            });

            await Task.Delay(5.Seconds());
            await client.StopAsync(10.Seconds());

            await startTask;

            return messageCount;
        }

        async Task<bool> ProccessNotification(GooglePlayNotification notification)
        {
            var subscription = await Repository.GetByPurchaseToken(notification.PurchaseToken);

            if (subscription == null)
            {
                var subscriptionInfo = await StoreConnector.GetSubscriptionInfo(notification.ToArgs());

                if (subscriptionInfo == null) return false;

                subscription = await Repository.AddSubscription(new Subscription
                {
                    Id = Guid.NewGuid().ToString(),
                    ProductId = notification.ProductId,
                    UserId = subscriptionInfo.UserId,
                    Platform = "GooglePlay",
                    TransactionId = subscriptionInfo.TransactionId,
                    ReceiptData = null,
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
                if (notification.State.IsAnyOf(GooglePlaySubscriptionState.Purchased, GooglePlaySubscriptionState.Renewed))
                    subscription.SubscriptionDate = notification.EventTime;
                else if (notification.State == GooglePlaySubscriptionState.Canceled)
                    subscription.CancellationDate = notification.EventTime;
                else if (notification.State.IsAnyOf(GooglePlaySubscriptionState.Expired, GooglePlaySubscriptionState.Revoked))
                    subscription.ExpirationDate = notification.EventTime;

                await Repository.UpdateSubscription(subscription);
            }

            await Repository.AddTransaction(new Transaction
            {
                Id = Guid.NewGuid().ToString(),
                SubscriptionId = subscription.Id,
                Platform = "GooglePlay",
                Date = notification.EventTime ?? LocalTime.UtcNow,
                Details = notification.OriginalData
            });

            return true;
        }

        SubscriberClient.ClientCreationSettings GetSettings()
        {
            var json = new JsonCredentialParameters
            {
                Type = JsonCredentialParameters.ServiceAccountCredentialType,
                ProjectId = Options.ProjectId,
                PrivateKeyId = Options.PrivateKeyId,
                PrivateKey = Options.PrivateKey,
                ClientEmail = Options.ClientEmail,
                ClientId = Options.ClientId
            }.ToJson(new JsonSerializerOptions { PropertyNamingPolicy = new SnakeCasePropertyNamingPolicy() });

            return new SubscriberClient.ClientCreationSettings(
                credentials: GoogleCredential.FromJson(json).ToChannelCredentials()
            );
        }
        class SnakeCasePropertyNamingPolicy : JsonNamingPolicy
        {
            public override string ConvertName(string name) => name.ToSnakeCase();
        }
    }
}
