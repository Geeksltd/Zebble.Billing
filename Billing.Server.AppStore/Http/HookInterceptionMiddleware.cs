namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Olive;

    class HookInterceptionMiddleware
    {
        readonly RequestDelegate Next;

        public HookInterceptionMiddleware(RequestDelegate next)
        {
            Next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task InvokeAsync(HttpContext context, AppStoreHookInterceptor hookInterceptor)
        {
            await hookInterceptor.Intercept(await context.Request.Body.ReadAllText());
            await context.Response.WriteAsync($"{nameof(AppStoreHookInterceptor)} executed.");

            await Next(context);
        }
    }
}
