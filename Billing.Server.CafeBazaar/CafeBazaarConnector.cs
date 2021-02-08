namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using Olive;
    using CafeBazaar.DeveloperApi;

    class CafeBazaarConnector : IStoreConnector
    {
        readonly CafeBazaarOptions Options;
        readonly CafeBazaarDeveloperService DeveloperService;

        public CafeBazaarConnector(IOptionsSnapshot<CafeBazaarOptions> options, CafeBazaarDeveloperService developerService)
        {
            Options = options.Value;
            DeveloperService = developerService;
        }

        public async Task<Subscription> GetUpToDateInfo(string productId, string purchaseToken)
        {
            var purchaseResult = await DeveloperService.ValidatePurchase(new CafeBazaarValidatePurchaseRequest
            {
                PackageName = Options.PackageName,
                ProductId = productId,
                PurchaseToken = purchaseToken
            });

            if (purchaseResult == null)
                return null;

            var subscriptionResult = await DeveloperService.ValidateSubscription(new CafeBazaarValidateSubscriptionRequest
            {
                PackageName = Options.PackageName,
                SubscriptionId = productId,
                PurchaseToken = purchaseToken
            });

            if (subscriptionResult == null)
                return null;

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
                OriginalTransactionId = null,
                SubscriptionDate = subscription.InitiationTime.DateTime,
                ExpirationDate = subscription.ValidUntil.DateTime,
                CancellationDate = purchase.PurchaseState == CafeBazaarPurchaseState.Refunded ? (DateTime?)LocalTime.Now : null,
                LastUpdate = LocalTime.Now,
                AutoRenews = subscription.AutoRenewing
            };
        }
    }
}
