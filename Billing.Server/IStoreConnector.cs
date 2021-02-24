namespace Zebble.Billing
{
    using System.Threading.Tasks;

    public interface IStoreConnector
    {
        Task<bool> VerifyPurchase(string productId, string receiptData);
        Task<SubscriptionInfo> GetUpToDateInfo(string productId, string purchaseToken);
    }
}
