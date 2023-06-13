namespace Zebble.Billing
{
    using Huawei.Hms.Iap.Entity;
    using System.Threading.Tasks;

    partial class BillingContext
    {
        internal Task<(PurchaseResult, string)> ProcessPurchase(IBillingUser user, InAppPurchaseData purchase)
            => ProcessPurchase(user, purchase.ToEventArgs());
    }
}
