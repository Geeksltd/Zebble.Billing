namespace Zebble.Billing
{
    using System.Threading.Tasks;

    public interface IQueueProcessor
    {
        Task<int> Process();
    }
}
