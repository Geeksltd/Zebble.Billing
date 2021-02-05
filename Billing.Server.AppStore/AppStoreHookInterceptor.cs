namespace Zebble.Billing
{
    using Microsoft.Extensions.Options;
    using System;
    using System.Threading.Tasks;

    class AppStoreHookInterceptor : IHookInterceptor
    {
        readonly AppStoreOptions options;

        public AppStoreHookInterceptor(IOptionsSnapshot<AppStoreOptions> options)
        {
            this.options = options.Value;
        }

        public Uri RelativeUri => options.HookInterceptorUri;

        public async Task Intercept(string body)
        {
            var notification = body.FromJson<AppStoreServerNotification>();
        }
    }
}
