﻿namespace Zebble.Billing
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

    class AppStoreLiveSubscriptionQuery : AppStorePlatform, ILiveSubscriptionQuery
    {
        readonly GooglePlayOptions _playOptions;
        readonly GooglePublisherOptions _publisherOptions;
        AndroidPublisherService _instance;

        public AppStoreLiveSubscriptionQuery(IOptionsSnapshot<GooglePlayOptions> playOptions, IOptionsSnapshot<GooglePublisherOptions> publisherOptions)
        {
            _playOptions = playOptions.Value;
            _publisherOptions = publisherOptions.Value;
        }

        public async Task<Subscription> GetUpToDateInfo(string productId, string purchaseToken)
        {
            var publisher = GetPublisherService();

            var result = await publisher.Purchases.Subscriptions.Get(_playOptions.PackageName, productId, purchaseToken).ExecuteAsync();

            if (result == null)
                return null;

            return CreateSubscription(purchaseToken, productId, result);
        }

        Subscription CreateSubscription(string purchaseToken, string productId, SubscriptionPurchase purchase)
        {
            return new Subscription
            {
                SubscriptionId = Guid.NewGuid(),
                ProductId = productId,
                UserId = purchase.EmailAddress,
                Platform = Platform,
                PurchaseToken = purchaseToken,
                DateSubscribed = purchase.StartTimeMillis.ToDateTime() ?? LocalTime.Now,
                ExpiryDate = purchase.ExpiryTimeMillis.ToDateTime() ?? LocalTime.Now,
                CancellationDate = purchase.UserCancellationTimeMillis.ToDateTime(),
                LastUpdated = LocalTime.Now,
                AutoRenews = purchase.AutoRenewing ?? false
            };
        }

        AndroidPublisherService GetPublisherService()
        {
            if (_instance != null) return _instance;

            var initializer = new BaseClientService.Initializer
            {
                HttpClientInitializer = CreateClientInitializer()
            };

            return _instance = new AndroidPublisherService(initializer);
        }

        IConfigurableHttpClientInitializer CreateClientInitializer()
        {
            return new ServiceAccountCredential(new ServiceAccountCredential.Initializer(_publisherOptions.ClientEmail)
            {
                ProjectId = _publisherOptions.ProjectId,
                Scopes = new[] { AndroidPublisherService.ScopeConstants.Androidpublisher }
            }.FromPrivateKey(_publisherOptions.PrivateKey));
        }
    }
}