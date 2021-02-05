namespace Zebble.Billing
{
    using Microsoft.Extensions.Options;
    using System;
    using System.Threading.Tasks;

    class AppStoreHookInterceptor
    {
        public async Task Intercept(string body)
        {
            var notification = body.FromJson<AppStoreServerNotification>();
        }
    }
}
