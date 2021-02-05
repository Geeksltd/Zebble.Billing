namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using Olive;
    using CafeBazaar.DeveloperApi;

    class CafeBazaarPlatformProvider : IPlatformProvider
    {
        readonly CafeBazaarOptions options;
        readonly CafeBazaarDeveloperService developerService;

        public string Platform => "CafeBazaar";

        public CafeBazaarPlatformProvider(IOptionsSnapshot<CafeBazaarOptions> options, CafeBazaarDeveloperService developerService)
        {
            this.options = options.Value;
            this.developerService = developerService;
        }

        public async Task<Subscription> GetUpToDateInfo(string productId, string purchaseToken)
        {
            var purchaseResult = await developerService.ValidatePurchase(new CafeBazaarValidatePurchaseRequest
            {
                PackageName = options.PackageName,
                ProductId = productId,
                PurchaseToken = purchaseToken
            });

            if (purchaseResult == null)
                return null;

            var subscriptionResult = await developerService.ValidateSubscription(new CafeBazaarValidateSubscriptionRequest
            {
                PackageName = options.PackageName,
                SubscriptionId = productId,
                PurchaseToken = purchaseToken
            });

            return CreateSubscription(productId, purchaseToken, purchaseResult, subscriptionResult);
        }

        Subscription CreateSubscription(string productId, string purchaseToken, CafeBazaarValidatePurchaseResult purchase, CafeBazaarValidateSubscriptionResult subscription)
        {
            return new Subscription
            {
                SubscriptionId = Guid.NewGuid().ToString(),
                ProductId = productId,
                UserId = purchase.DeveloperPayload,
                Platform = "CafeBazaar",
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
