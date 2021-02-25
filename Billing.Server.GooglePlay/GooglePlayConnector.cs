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

    class GooglePlayConnector : IStoreConnector, IDisposable
    {
        readonly GooglePlayOptions Options;
        AndroidPublisherService Instance;

        public GooglePlayConnector(IOptionsSnapshot<GooglePlayOptions> options)
        {
            Options = options.Value;
        }

        public Task<bool> VerifyPurchase(VerifyPurchaseArgs args)
        {
            return Task.FromResult(true);
        }

        public async Task<SubscriptionInfo> GetSubscriptionInfo(SubscriptionInfoArgs args)
        {
            var publisher = GetPublisherService();

            var result = await publisher.Purchases.Subscriptions.Get(Options.PackageName, args.ProductId, args.PurchaseToken).ExecuteAsync();

            if (result == null)
                return null;

            return CreateSubscription(result);
        }

        public void Dispose() => Instance?.Dispose();

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
            return new ServiceAccountCredential(new ServiceAccountCredential.Initializer(Options.ClientEmail)
            {
                ProjectId = Options.ProjectId,
                Scopes = new[] { AndroidPublisherService.ScopeConstants.Androidpublisher }
            }.FromPrivateKey(Options.PrivateKey));
        }
    }
}
