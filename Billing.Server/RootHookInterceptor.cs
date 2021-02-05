namespace Zebble.Billing
{
    using System.Threading.Tasks;

    class RootHookInterceptor : IRootHookInterceptor
    {
        readonly IPlatformProvider<IHookInterceptor> _hookInterceptorProvider;

        public RootHookInterceptor(IPlatformProvider<IHookInterceptor> hookInterceptorProvider)
        {
            _hookInterceptorProvider = hookInterceptorProvider;
        }

        public Task Intercept(SubscriptionPlatform platform)
        {
            return _hookInterceptorProvider[platform].Intercept();
        }
    }
}
