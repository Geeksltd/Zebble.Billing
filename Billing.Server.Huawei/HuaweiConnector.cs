namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Huawei.DeveloperApi;

    public class HuaweiConnector : IStoreConnector
    {
        readonly HuaweiDeveloperService DeveloperService;

        public HuaweiConnector(HuaweiDeveloperService developerService)
            => DeveloperService = developerService ?? throw new ArgumentNullException(nameof(developerService));

        public async Task<SubscriptionInfo> GetSubscriptionInfo(SubscriptionInfoArgs args)
        {
            var result = await DeveloperService.ValidateSubscription(new HuaweiValidateSubscriptionRequest
            {
                SubscriptionId = args.SubscriptionId,
                PurchaseToken = args.PurchaseToken
            });

            if (result is null) return SubscriptionInfo.NotFound;

            return CreateSubscription(args.UserId, result);
        }

        static SubscriptionInfo CreateSubscription(string userId, HuaweiValidateSubscriptionResult result)
        {
            return new SubscriptionInfo
            {
                UserId = userId,
                SubscriptionId = result.InappPurchaseData.SubscriptionId,
                TransactionId = result.InappPurchaseData.OrderId,
                SubscriptionDate = result.InappPurchaseData.PurchaseTime?.DateTime,
                ExpirationDate = result.InappPurchaseData.ExpirationDate?.DateTime,
                AutoRenews = result.InappPurchaseData.AutoRenewing
            };
        }
    }
}
