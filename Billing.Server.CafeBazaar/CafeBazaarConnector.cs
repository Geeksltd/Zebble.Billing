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
            Options = options.Value ?? throw new ArgumentNullException(nameof(options));
            DeveloperService = developerService ?? throw new ArgumentNullException(nameof(developerService));
        }

        public async Task<SubscriptionInfo> GetSubscriptionInfo(SubscriptionInfoArgs args)
        {
            var purchaseResult = await DeveloperService.ValidatePurchase(new CafeBazaarValidatePurchaseRequest
            {
                PackageName = args.PackageName,
                ProductId = args.ProductId,
                PurchaseToken = args.PurchaseToken
            });

            if (purchaseResult is null) return null;

            var subscriptionResult = await DeveloperService.ValidateSubscription(new CafeBazaarValidateSubscriptionRequest
            {
                PackageName = args.PackageName,
                SubscriptionId = args.ProductId,
                PurchaseToken = args.PurchaseToken
            });

            if (subscriptionResult is null) return null;

            return CreateSubscription(purchaseResult, subscriptionResult);
        }

        static SubscriptionInfo CreateSubscription(
            CafeBazaarValidatePurchaseResult purchase,
            CafeBazaarValidateSubscriptionResult subscription)
        {
            return new SubscriptionInfo
            {
                SubscriptionDate = subscription.InitiationTime.DateTime,
                ExpirationDate = subscription.ValidUntil.DateTime,
                CancellationDate = purchase.PurchaseState == CafeBazaarPurchaseState.Refunded ? LocalTime.UtcNow : null,
                AutoRenews = subscription.AutoRenewing
            };
        }
    }
}
