namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using Google.Apis.Auth.OAuth2;
    using Google.Cloud.PubSub.V1;
    using Grpc.Auth;
    using Olive;

    public class GooglePlayQueueProcessor : IQueueProcessor
    {
        readonly GooglePubSubOptions _options;
        readonly ISubscriptionRepository _subscriptionRepository;
        readonly ITransactionRepository _transactionRepository;
        readonly GooglePublisherApi _publisherApi;

        public SubscriptionPlatform Platform => SubscriptionPlatform.GooglePlay;

        public GooglePlayQueueProcessor(
            IOptionsSnapshot<GooglePubSubOptions> options,
            ISubscriptionRepository subscriptionRepository,
            ITransactionRepository transactionRepository,
            GooglePublisherApi publisherApi
        )
        {
            _options = options.Value;
            _subscriptionRepository = subscriptionRepository;
            _transactionRepository = transactionRepository;
            _publisherApi = publisherApi;
        }

        public async Task<int> Process()
        {
            var handled = 0;

            var name = new SubscriptionName(_options.ProjectId, _options.SubscriptionId);
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
            var subscription = await _subscriptionRepository.GetByPurchaseToken(notification.PurchaseToken);

            if (subscription == null)
            {
                subscription = await _publisherApi.GetSubscription(notification.PurchaseToken, notification.ProductId);

                if (subscription == null)
                    return false;

                subscription = await _subscriptionRepository.Add(subscription);
            }
            else
            {
                if (notification.State == GoogleNotification.SubscriptionState.Canceled)
                    subscription.CancellationDate = notification.EventTime;
                else if (notification.State == GoogleNotification.SubscriptionState.Expired)
                    subscription.ExpiryDate = notification.EventTime;

                await _subscriptionRepository.Update(subscription);
            }

            await _transactionRepository.Save(new Transaction
            {
                TransactionId = Guid.NewGuid(),
                SubscriptionId = subscription.SubscriptionId,
                Platform = SubscriptionPlatform.GooglePlay,
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
                ProjectId = _options.ProjectId,
                PrivateKeyId = _options.PrivateKeyId,
                PrivateKey = _options.PrivateKey,
                ClientEmail = _options.ClientEmail,
                ClientId = _options.ClientId
            }.ToJson();

            return new SubscriberClient.ClientCreationSettings(
                credentials: GoogleCredential.FromJson(json).ToChannelCredentials()
            );
        }
    }
}
