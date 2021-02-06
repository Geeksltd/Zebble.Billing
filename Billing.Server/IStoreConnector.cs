namespace Zebble.Billing
{
    using System.Threading.Tasks;

    public interface IStoreConnector
    {
        Task<Subscription> GetUpToDateInfo(string productId, string purchaseToken);
    }
}
