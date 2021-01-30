namespace Zebble.Billing
{
    using Microsoft.Extensions.Options;
    using System.Threading.Tasks;

    class CafeBazaarQueueProcessor : IQueueProcessor
    {
        readonly CafeBazaarOptions _options;

        public SubscriptionPlatform Platform => SubscriptionPlatform.CafeBazaar;

        public CafeBazaarQueueProcessor(IOptionsSnapshot<CafeBazaarOptions> options)
        {
            _options = options.Value;
        }

        public Task<int> Process()
        {
            return Task.FromResult(0);
        }
    }
}
