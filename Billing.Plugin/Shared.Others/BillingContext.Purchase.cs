namespace Zebble.Billing
{
    using Olive;
    using Plugin.InAppBilling;
    using System.Threading.Tasks;

    partial class BillingContext
    {
        internal Task<(PurchaseResult, string)> ProcessPurchase(IBillingUser user, InAppBillingPurchase purchase)
            => ProcessPurchase(user, purchase.ToEventArgs());
    }
}
