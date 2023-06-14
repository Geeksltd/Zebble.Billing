namespace Zebble.Billing
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Huawei.Hmf.Extensions;
    using Huawei.Hms.Iap.Entity;
    using Olive;

    class RestoreSubscriptionCommand : StoreCommandBase<bool>
    {
        protected override async Task<bool> DoExecute(IBillingUser user)
        {
            try
            {
                var subscriptions = await GetPurchasesAsync(ProductType.Subscription);
                var inAppPurchases = await GetPurchasesAsync(ProductType.InAppPurchase);

                var purchases = subscriptions
                    .Concat(inAppPurchases)
                    .Where(x => x.PurchaseStatus.IsAnyOf(OrderStatusCode.OrderStateSuccess, OrderStatusCode.OrderProductOwned))
                    .Where(x => x.PurchaseToken.HasValue())
                    .Distinct(x => new { x.OrderID, x.ProductId }).ToArray();

                if (purchases.None()) return false;

                foreach (var purchase in purchases)
                {
                    var (result, _) = await BillingContext.Current.ProcessPurchase(user, purchase);
                }

                return true;
            }
            catch (Exception ex)
            {
                Log.For(this).Error(ex);
                return false;
            }
        }

        async Task<InAppPurchaseData[]> GetPurchasesAsync(ProductType productType)
        {
            var request = new OwnedPurchasesReq
            {
                PriceType = productType.GetPriceType()
            };

            var purchases = await Billing.ObtainOwnedPurchases(request)
                .AsAsync<OwnedPurchasesResult>();

            return purchases.InAppPurchaseDataList
                .OrEmpty()
                .Select(x => new InAppPurchaseData(x))
                .ToArray();
        }
    }
}