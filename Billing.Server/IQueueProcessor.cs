namespace Zebble.Billing
{
    using System.Threading.Tasks;

    public interface IQueueProcessor : IPlatformAware
    {
        Task<int> Process();
    }
}
