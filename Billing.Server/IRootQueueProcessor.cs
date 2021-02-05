namespace Zebble.Billing
{
    using System.Threading.Tasks;

    public interface IRootQueueProcessor
    {
        Task<int> ProcessAll();
        Task<int> Process(string platform);
    }
}
