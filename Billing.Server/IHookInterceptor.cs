namespace Zebble.Billing
{
    using System.Threading.Tasks;

    public interface IHookInterceptor : IPlatformAware
    {
        Task Intercept();
    }
}
