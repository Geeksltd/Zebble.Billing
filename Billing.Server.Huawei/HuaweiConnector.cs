namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;

    public class HuaweiConnector : IStoreConnector
    {
        readonly HuaweiOptions Options;
        readonly HuaweiDeveloperService DeveloperService;

        public HuaweiConnector(IOptionsSnapshot<HuaweiOptions> options, HuaweiDeveloperService developerService)
        {
            Options = options.Value ?? throw new ArgumentNullException(nameof(options));
            DeveloperService = developerService ?? throw new ArgumentNullException(nameof(developerService));
        }

        public async Task<SubscriptionInfo> GetSubscriptionInfo(SubscriptionInfoArgs args)
        {
            var purchaseResult = await DeveloperService.ValidatePurchase(new HuaweiValidatePurchaseRequest
            {
                PackageName = Options.PackageName,
                PublicKey = Options.PublicKey,
                ProductId = args.ProductId,
                PurchaseToken = args.PurchaseToken
            });

            if (purchaseResult is null) return SubscriptionInfo.NotFound;

            return CreateSubscription(args.UserId, purchaseResult);
        }

        SubscriptionInfo CreateSubscription(string userId, HuaweiValidatePurchaseResult purchase)
        {
            return new SubscriptionInfo
            {
                UserId = userId,
                SubscriptionDate = purchase.SubscriptionDate,
                ExpirationDate = purchase.ExpirationDate,
                CancellationDate = purchase.CancellationDate,
                AutoRenews = purchase.AutoRenews
            };
        }
    }
}
