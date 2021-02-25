namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
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
            var handled = 0;

            var name = new SubscriptionName(Options.ProjectId, Options.SubscriptionId);
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

        async Task<bool> ProccessNotification(GooglePlayNotification notification)
        {
            var subscription = await Repository.GetByPurchaseToken(notification.PurchaseToken);

            if (subscription == null)
            {
                var subscriptionInfo = await StoreConnector.GetSubscriptionInfo(notification.ToArgs());

                if (subscriptionInfo == null) return false;

                subscription = await Repository.AddSubscription(new Subscription
                {
                    SubscriptionId = Guid.NewGuid().ToString(),
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
                    LastUpdate = LocalTime.Now,
                    AutoRenews = subscriptionInfo.AutoRenews
                });
            }
            else
            {
                if (notification.State == GooglePlaySubscriptionState.Canceled)
                    subscription.CancellationDate = notification.EventTime;
                else if (notification.State == GooglePlaySubscriptionState.Expired)
                    subscription.ExpirationDate = notification.EventTime;

                await Repository.UpdateSubscription(subscription);
            }

            await Repository.AddTransaction(new Transaction
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
                ProjectId = Options.ProjectId,
                PrivateKeyId = Options.PrivateKeyId,
                PrivateKey = Options.PrivateKey,
                ClientEmail = Options.ClientEmail,
                ClientId = Options.ClientId
            }.ToJson();

            return new SubscriberClient.ClientCreationSettings(
                credentials: GoogleCredential.FromJson(json).ToChannelCredentials()
            );
        }
    }
}
