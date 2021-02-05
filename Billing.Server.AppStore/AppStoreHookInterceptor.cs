namespace Zebble.Billing
{
    using System.Threading.Tasks;

    class AppStoreHookInterceptor : AppStorePlatform, IHookInterceptor
    {
        readonly ISubscriptionRepository subscriptionRepository;
        readonly ITransactionRepository transactionRepository;
        readonly ILiveSubscriptionQuery liveSubscriptionQuery;

        public AppStoreHookInterceptor(
            ISubscriptionRepository subscriptionRepository,
            ITransactionRepository transactionRepository,
           IPlatformProvider<ILiveSubscriptionQuery> liveSubscriptionQueryProvider
        )
        {
            this.subscriptionRepository = subscriptionRepository;
            this.transactionRepository = transactionRepository;
            this.liveSubscriptionQuery = liveSubscriptionQueryProvider[Platform];
        }

        public async Task Intercept(string body)
        {
            var notification = body.FromJson<AppStoreServerNotification>();
        }
    }
}
