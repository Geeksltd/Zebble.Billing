namespace Zebble.Billing
{
    using System.Threading.Tasks;

    public abstract class SubscriptionProcessorBase : ISubscriptionProcessor
    {
        public abstract Task<bool> Refresh();
    }
}
