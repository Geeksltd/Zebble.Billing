namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using Google.Apis.Auth.OAuth2;
    using Google.Apis.AndroidPublisher.v3;
    using Google.Apis.Services;
    using Google.Apis.Http;
    using Google.Apis.AndroidPublisher.v3.Data;
    using Olive;

    class GooglePlayConnector : IStoreConnector
    {
        readonly GooglePlayOptions PlayOptions;
        readonly GooglePublisherOptions PublisherOptions;
        AndroidPublisherService instance;

        public GooglePlayConnector(IOptionsSnapshot<GooglePlayOptions> playOptions, IOptionsSnapshot<GooglePublisherOptions> publisherOptions)
        {
            PlayOptions = playOptions.Value;
            PublisherOptions = publisherOptions.Value;
        }

        public async Task<Subscription> GetUpToDateInfo(string productId, string purchaseToken)
        {
            var publisher = GetPublisherService();

            var result = await publisher.Purchases.Subscriptions.Get(PlayOptions.PackageName, productId, purchaseToken).ExecuteAsync();

            if (result == null)
                return null;

            return CreateSubscription(productId, purchaseToken, result);
        }

        Subscription CreateSubscription(string productId, string purchaseToken, SubscriptionPurchase purchase)
        {
            return new Subscription
            {
                SubscriptionId = Guid.NewGuid().ToString(),
                ProductId = productId,
                UserId = purchase.EmailAddress,
                Platform = "GooglePlay",
                PurchaseToken = purchaseToken,
                OriginalTransactionId = purchase.OrderId,
                DateSubscribed = purchase.StartTimeMillis.ToDateTime() ?? LocalTime.Now,
                ExpiryDate = purchase.ExpiryTimeMillis.ToDateTime() ?? LocalTime.Now,
                CancellationDate = purchase.UserCancellationTimeMillis.ToDateTime(),
                LastUpdated = LocalTime.Now,
                AutoRenews = purchase.AutoRenewing ?? false
            };
        }

        AndroidPublisherService GetPublisherService()
        {
            if (instance != null) return instance;

            var initializer = new BaseClientService.Initializer
            {
                HttpClientInitializer = CreateClientInitializer()
            };

            return instance = new AndroidPublisherService(initializer);
        }

        IConfigurableHttpClientInitializer CreateClientInitializer()
        {
            return new ServiceAccountCredential(new ServiceAccountCredential.Initializer(PublisherOptions.ClientEmail)
            {
                ProjectId = PublisherOptions.ProjectId,
                Scopes = new[] { AndroidPublisherService.ScopeConstants.Androidpublisher }
            }.FromPrivateKey(PublisherOptions.PrivateKey));
        }
    }
}
