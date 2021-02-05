namespace Zebble.Billing
{
    using System.Threading.Tasks;

    public interface IRootHookInterceptor
    {
        Task Intercept(SubscriptionPlatform platform, string body);
    }
}
