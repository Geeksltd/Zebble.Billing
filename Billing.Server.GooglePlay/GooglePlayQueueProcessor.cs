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
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    class GooglePlayQueueProcessor
    {
        readonly ILogger<GooglePlayQueueProcessor> Logger;
        readonly GooglePlayOptions Options;
        readonly IServiceProvider Services;
        readonly GooglePlayConnector StoreConnector;

        public GooglePlayQueueProcessor(ILogger<GooglePlayQueueProcessor> logger, IOptionsSnapshot<GooglePlayOptions> options, IServiceProvider services, GooglePlayConnector storeConnector)
        {
            Logger = logger;
            Options = options.Value;
            Services = services;
            StoreConnector = storeConnector;
        }

        public async Task<int> Process()
        {
            var name = new SubscriptionName(Options.ProjectId, Options.SubscriptionId);
            var client = await SubscriberClient.CreateAsync(name, GetSettings(), new SubscriberClient.Settings { });
            var messageCount = 0;

            var startTask = client.StartAsync(async (message, _) =>
            {
                var notification = message.ToNotification();

                Interlocked.Increment(ref messageCount);

                if (!notification.IsTest) await ProccessNotification(notification);

                return SubscriberClient.Reply.Ack;
            });

            await Task.Delay(5.Seconds());
            await client.StopAsync(CancellationToken.None);

            await startTask;

            Logger.Debug($"{messageCount} queue messages are processed.");

            return messageCount;
        }

        async Task ProccessNotification(GooglePlayNotification notification)
        {
            try
            {
                using var scope = Services.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<ISubscriptionRepository>();

                var subscription = await repository.GetByPurchaseToken(notification.PurchaseToken);

                if (subscription is null)
                {
                    var subscriptionInfo = await StoreConnector.GetSubscriptionInfo(notification.ToArgs());
                    if (subscriptionInfo.Status != SubscriptionQueryStatus.Succeeded) return;

                    subscription = await repository.AddSubscription(new Subscription
                    {
                        Id = Guid.NewGuid().ToString(),
                        ProductId = notification.ProductId,
                        UserId = subscriptionInfo.UserId,
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

                if (notification.State.IsAnyOf(GooglePlaySubscriptionState.Purchased, GooglePlaySubscriptionState.Renewed))
                    subscription.SubscriptionDate = notification.EventTime;
                else if (notification.State == GooglePlaySubscriptionState.Canceled)
                    subscription.CancellationDate = notification.EventTime;
                else if (notification.State.IsAnyOf(GooglePlaySubscriptionState.Expired, GooglePlaySubscriptionState.Revoked))
                    subscription.ExpirationDate = notification.EventTime;

                await repository.UpdateSubscription(subscription);

                await repository.AddTransaction(new Transaction
                {
                    Id = Guid.NewGuid().ToString(),
                    SubscriptionId = subscription.Id,
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

        SubscriberClient.ClientCreationSettings GetSettings()
        {
            var json = new JsonCredentialParameters
            {
                Type = JsonCredentialParameters.ServiceAccountCredentialType,
                ProjectId = Options.ProjectId,
                PrivateKeyId = Options.PubSubPrivateKeyId ?? Options.PrivateKeyId,
                PrivateKey = Options.PubSubPrivateKey ?? Options.PrivateKey,
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
