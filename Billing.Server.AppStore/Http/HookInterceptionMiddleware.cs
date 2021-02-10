namespace Zebble.Billing
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Olive;

    public class HookInterceptionMiddleware
    {
        public async Task InvokeAsync(HttpContext context, AppStoreHookInterceptor hookInterceptor)
        {
            await hookInterceptor.Intercept(await context.Request.Body.ReadAllText());
            await context.Response.WriteAsync($"{nameof(AppStoreHookInterceptor)} executed.");
        }
    }
}
