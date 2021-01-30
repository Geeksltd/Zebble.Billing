namespace Zebble.Billing
{
    using System.Threading.Tasks;

    public interface ILiveSubscriptionQuery : IPlatformAware
    {
        Task<Subscription> GetUpToDateInfo(string productId, string purchaseToken);
    }
}
