namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;

    public interface IHookInterceptor : IPlatformAware
    {
        Uri RelativeUri { get; }
        Task Intercept(string body);
    }
}
