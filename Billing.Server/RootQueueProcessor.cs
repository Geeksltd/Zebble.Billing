namespace Zebble.Billing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Olive;

    class RootQueueProcessor : IRootQueueProcessor
    {
        readonly IDictionary<SubscriptionPlatform, IQueueProcessor> _queueProcessors;

        public RootQueueProcessor(IEnumerable<IQueueProcessor> queueProcessors)
        {
            _queueProcessors = queueProcessors.ToDictionary(x => x.Platform, x => x);
        }

        public Task<int> ProcessAll()
        {
            var processes = _queueProcessors.Values.Select(x => x.Process());

            return Task.WhenAll(processes).ContinueWith(x => x.GetAlreadyCompletedResult().Sum());
        }

        public Task<int> Process(SubscriptionPlatform platform)
        {
            return _queueProcessors[platform]?.Process() ?? throw new NotSupportedException($"Queue processing isn't supported by {platform}.");
        }
    }
}
