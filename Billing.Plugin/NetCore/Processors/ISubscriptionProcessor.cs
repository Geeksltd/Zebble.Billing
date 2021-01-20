namespace Zebble.Billing
{
    using System.Threading.Tasks;

    public interface ISubscriptionProcessor
    {
        Task<bool> Refresh();
    }
}
