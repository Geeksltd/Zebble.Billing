namespace Zebble.Billing
{
    using System.Threading.Tasks;

    public interface ISubscriptionProcessor : IPlatformAware
    {
        Task<bool> Refresh();
    }
}
