namespace Zebble.Billing
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    class AppStoreHookInterceptionMiddleware
    {
        public AppStoreHookInterceptionMiddleware(RequestDelegate _) { }

        public async Task InvokeAsync(HttpContext context, AppStoreHookInterceptor hookInterceptor)
        {
            var notification = await context.Request.Body.ConvertTo<AppStoreNotification>();

            await hookInterceptor.Intercept(notification);
        }
    }
}
