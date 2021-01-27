namespace Zebble.Billing
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;

    public class GooglePlaySubscriptionProcessor : ISubscriptionProcessor
    {
        readonly GooglePlayOptions _options;

        public SubscriptionPlatform Platform => SubscriptionPlatform.GooglePlay;

        public GooglePlaySubscriptionProcessor(IOptions<GooglePlayOptions> options)
        {
            _options = options.Value;
        }

        public Task<bool> Refresh()
        {
            return Task.FromResult(false);
        }
    }
}
