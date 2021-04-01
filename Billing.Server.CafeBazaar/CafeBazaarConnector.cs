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

        public Task<PurchaseVerificationResult> VerifyPurchase(VerifyPurchaseArgs args)
        {
            return Task.FromResult(PurchaseVerificationResult.Verified);
        }

        public async Task<SubscriptionInfo> GetSubscriptionInfo(SubscriptionInfoArgs args)
        {
            var purchaseResult = await DeveloperService.ValidatePurchase(new CafeBazaarValidatePurchaseRequest
            {
                PackageName = Options.PackageName,
                ProductId = args.ProductId,
                PurchaseToken = args.PurchaseToken
            });

            if (purchaseResult is null) return null;

            var subscriptionResult = await DeveloperService.ValidateSubscription(new CafeBazaarValidateSubscriptionRequest
            {
                PackageName = Options.PackageName,
                SubscriptionId = args.ProductId,
                PurchaseToken = args.PurchaseToken
            });

            if (subscriptionResult is null) return null;

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
                CancellationDate = purchase.PurchaseState == CafeBazaarPurchaseState.Refunded ? (DateTime?)LocalTime.UtcNow : null,
                AutoRenews = subscription.AutoRenewing
            };
        }
    }
}
