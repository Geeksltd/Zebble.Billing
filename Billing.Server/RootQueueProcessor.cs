namespace Zebble.Billing
{
    using System.Linq;
    using System.Threading.Tasks;
    using Olive;

    class RootQueueProcessor : IRootQueueProcessor
    {
        readonly IPlatformProvider<IQueueProcessor> _queueProcessorProvider;

        public RootQueueProcessor(IPlatformProvider<IQueueProcessor> queueProcessorProvider)
        {
            _queueProcessorProvider = queueProcessorProvider;
        }

        public Task<int> ProcessAll()
        {
            var processes = _queueProcessorProvider.Select(x => x.Process());

            return Task.WhenAll(processes).ContinueWith(x => x.GetAlreadyCompletedResult().Sum());
        }

        public Task<int> Process(SubscriptionPlatform platform)
        {
            return _queueProcessorProvider[platform].Process();
        }
    }
}
