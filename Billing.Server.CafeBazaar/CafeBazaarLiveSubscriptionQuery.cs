namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using Olive;
    using CafeBazaar.DeveloperApi;

    class CafeBazaarLiveSubscriptionQuery : ILiveSubscriptionQuery
    {
        readonly CafeBazaarOptions _options;
        readonly CafeBazaarDeveloperService _developerService;

        public SubscriptionPlatform Platform => SubscriptionPlatform.GooglePlay;

        public CafeBazaarLiveSubscriptionQuery(IOptionsSnapshot<CafeBazaarOptions> options, CafeBazaarDeveloperService developerService)
        {
            _options = options.Value;
            _developerService = developerService;
        }

        public async Task<Subscription> GetUpToDateInfo(string productId, string purchaseToken)
        {
            var purchaseResult = await _developerService.ValidatePurchase(new CafeBazaarValidatePurchaseRequest
            {
                PackageName = _options.PackageName,
                ProductId = productId,
                PurchaseToken = purchaseToken
            });

            if (purchaseResult == null)
                return null;

            var subscriptionResult = await _developerService.ValidateSubscription(new CafeBazaarValidateSubscriptionRequest
            {
                PackageName = _options.PackageName,
                SubscriptionId = productId,
                PurchaseToken = purchaseToken
            });

            return CreateSubscription(purchaseToken, productId, purchaseResult, subscriptionResult);
        }

        Subscription CreateSubscription(string purchaseToken, string productId, CafeBazaarValidatePurchaseResult purchase, CafeBazaarValidateSubscriptionResult subscription)
        {
            return new Subscription
            {
                SubscriptionId = Guid.NewGuid(),
                ProductId = productId,
                UserId = purchase.DeveloperPayload,
                Platform = Platform,
                PurchaseToken = purchaseToken,
                DateSubscribed = subscription.InitiationTime.DateTime,
                ExpiryDate = subscription.ValidUntil.DateTime,
                CancellationDate = purchase.PurchaseState == CafeBazaarPurchaseState.Refunded ? (DateTime?)LocalTime.Now : null,
                LastUpdated = LocalTime.Now,
                AutoRenews = subscription.AutoRenewing
            };
        }
    }
}
