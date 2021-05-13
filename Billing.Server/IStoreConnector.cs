namespace Zebble.Billing
{
    using System.Threading.Tasks;

    public interface IStoreConnector
    {
        Task<SubscriptionInfo> GetSubscriptionInfo(SubscriptionInfoArgs args);
    }
}
