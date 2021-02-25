namespace Zebble.Billing
{
    using System.Threading.Tasks;

    public interface IStoreConnector
    {
        Task<bool> VerifyPurchase(VerifyPurchaseArgs args);
        Task<SubscriptionInfo> GetSubscriptionInfo(SubscriptionInfoArgs args);
    }
}
