namespace Zebble.Billing
{
    using System.Threading.Tasks;

    public interface IStoreConnector
    {
        Task<PurchaseVerificationResult> VerifyPurchase(VerifyPurchaseArgs args);
        Task<SubscriptionInfo> GetSubscriptionInfo(SubscriptionInfoArgs args);
    }
}
