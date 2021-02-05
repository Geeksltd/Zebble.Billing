namespace Zebble.Billing
{
    using System.Linq;
    using System.Threading.Tasks;
    using Olive;

    class RootQueueProcessor : IRootQueueProcessor
    {
        readonly IPlatformProvider<IQueueProcessor> queueProcessorProvider;

        public RootQueueProcessor(IPlatformProvider<IQueueProcessor> queueProcessorProvider)
        {
            this.queueProcessorProvider = queueProcessorProvider;
        }

        public Task<int> ProcessAll()
        {
            var processes = queueProcessorProvider.Select(x => x.Process());

            return Task.WhenAll(processes).ContinueWith(x => x.GetAlreadyCompletedResult().Sum());
        }

        public Task<int> Process(string platform)
        {
            return queueProcessorProvider[platform].Process();
        }
    }
}
