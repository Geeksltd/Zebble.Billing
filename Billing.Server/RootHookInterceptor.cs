namespace Zebble.Billing
{
    using System.Threading.Tasks;

    class RootHookInterceptor : IRootHookInterceptor
    {
        readonly IPlatformProvider<IHookInterceptor> hookInterceptorProvider;

        public RootHookInterceptor(IPlatformProvider<IHookInterceptor> hookInterceptorProvider)
        {
            this.hookInterceptorProvider = hookInterceptorProvider;
        }

        public Task Intercept(string platform, string body)
        {
            return hookInterceptorProvider[platform].Intercept(body);
        }
    }
}
