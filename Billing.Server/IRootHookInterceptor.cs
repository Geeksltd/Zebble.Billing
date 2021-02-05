namespace Zebble.Billing
{
    using System.Threading.Tasks;

    public interface IRootHookInterceptor
    {
        Task Intercept(string platform, string body);
    }
}
