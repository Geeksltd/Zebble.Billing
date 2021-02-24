namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using Olive;
    using CafeBazaar.DeveloperApi;

    public class CafeBazaarConnector : IStoreConnector
    {
        readonly CafeBazaarOptions Options;
        readonly CafeBazaarDeveloperService DeveloperService;

        public CafeBazaarConnector(IOptionsSnapshot<CafeBazaarOptions> options, CafeBazaarDeveloperService developerService)
        {
            Options = options.Value;
            DeveloperService = developerService;
        }

        public Task<bool> VerifyPurchase(string productId, string receiptData)
        {
            return Task.FromResult(true);
        }

        public async Task<SubscriptionInfo> GetUpToDateInfo(string productId, string purchaseToken)
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

            return CreateSubscription(purchaseResult, subscriptionResult);
        }

        SubscriptionInfo CreateSubscription(CafeBazaarValidatePurchaseResult purchase, CafeBazaarValidateSubscriptionResult subscription)
        {
            return new SubscriptionInfo
            {
                UserId = purchase.DeveloperPayload,
                TransactionId = null,
                SubscriptionDate = subscription.InitiationTime.DateTime,
                ExpirationDate = subscription.ValidUntil.DateTime,
                CancellationDate = purchase.PurchaseState == CafeBazaarPurchaseState.Refunded ? (DateTime?)LocalTime.Now : null,
                AutoRenews = subscription.AutoRenewing
            };
        }
    }
}
