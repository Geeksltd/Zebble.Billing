namespace Zebble.Billing
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Olive;

    class RootQueueProcessor : IRootQueueProcessor
    {
        readonly IPlatformSpecificProvider<IQueueProcessor> _queueProcessorProvider;

        public RootQueueProcessor(IPlatformSpecificProvider<IQueueProcessor> queueProcessorProvider)
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
            return _queueProcessorProvider[platform]?.Process() ?? throw new NotSupportedException($"Queue processing isn't supported by {platform}.");
        }
    }
}
