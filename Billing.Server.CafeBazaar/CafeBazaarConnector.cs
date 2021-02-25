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

        public Task<bool> VerifyPurchase(VerifyPurchaseArgs args)
        {
            return Task.FromResult(true);
        }

        public async Task<SubscriptionInfo> GetSubscriptionInfo(SubscriptionInfoArgs args)
        {
            var purchaseResult = await DeveloperService.ValidatePurchase(new CafeBazaarValidatePurchaseRequest
            {
                PackageName = Options.PackageName,
                ProductId = args.ProductId,
                PurchaseToken = args.PurchaseToken
            });

            if (purchaseResult == null)
                return null;

            var subscriptionResult = await DeveloperService.ValidateSubscription(new CafeBazaarValidateSubscriptionRequest
            {
                PackageName = Options.PackageName,
                SubscriptionId = args.ProductId,
                PurchaseToken = args.PurchaseToken
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
