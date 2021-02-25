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
        AndroidPublisherService Instance;

        public GooglePlayConnector(IOptionsSnapshot<GooglePlayOptions> playOptions, IOptionsSnapshot<GooglePublisherOptions> publisherOptions)
        {
            PlayOptions = playOptions.Value;
            PublisherOptions = publisherOptions.Value;
        }

        public Task<bool> VerifyPurchase(VerifyPurchaseArgs args)
        {
            return Task.FromResult(true);
        }

        public async Task<SubscriptionInfo> GetSubscriptionInfo(SubscriptionInfoArgs args)
        {
            var publisher = GetPublisherService();

            var result = await publisher.Purchases.Subscriptions.Get(PlayOptions.PackageName, args.ProductId, args.PurchaseToken).ExecuteAsync();

            if (result == null)
                return null;

            return CreateSubscription(result);
        }

        SubscriptionInfo CreateSubscription(SubscriptionPurchase purchase)
        {
            return new SubscriptionInfo
            {
                UserId = purchase.EmailAddress,
                TransactionId = purchase.OrderId,
                SubscriptionDate = purchase.StartTimeMillis.ToDateTime() ?? LocalTime.Now,
                ExpirationDate = purchase.ExpiryTimeMillis.ToDateTime() ?? LocalTime.Now,
                CancellationDate = purchase.UserCancellationTimeMillis.ToDateTime(),
                AutoRenews = purchase.AutoRenewing ?? false
            };
        }

        AndroidPublisherService GetPublisherService()
        {
            if (Instance != null) return Instance;

            var initializer = new BaseClientService.Initializer
            {
                HttpClientInitializer = CreateClientInitializer()
            };

            return Instance = new AndroidPublisherService(initializer);
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
