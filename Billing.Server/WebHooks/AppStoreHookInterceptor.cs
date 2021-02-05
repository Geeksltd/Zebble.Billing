namespace Zebble.Billing
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;

    class AppStoreHookInterceptor : AppStorePlatform, IHookInterceptor
    {
        readonly GooglePubSubOptions _options;
        readonly ISubscriptionRepository _subscriptionRepository;
        readonly ITransactionRepository _transactionRepository;
        readonly ILiveSubscriptionQuery _liveSubscriptionQuery;

        public AppStoreHookInterceptor(
            IOptionsSnapshot<GooglePubSubOptions> options,
            ISubscriptionRepository subscriptionRepository,
            ITransactionRepository transactionRepository,
           IPlatformProvider<ILiveSubscriptionQuery> liveSubscriptionQueryProvider
        )
        {
            _options = options.Value;
            _subscriptionRepository = subscriptionRepository;
            _transactionRepository = transactionRepository;
            _liveSubscriptionQuery = liveSubscriptionQueryProvider[Platform];
        }

        public async Task Intercept()
        {
        }
    }
}
