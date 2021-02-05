namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Options;
    using Olive;

    class HookInterceptionMiddleware
    {
        readonly RequestDelegate next;

        public HookInterceptionMiddleware(RequestDelegate next)
        {
            this.next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task InvokeAsync(HttpContext context, IOptionsSnapshot<AppStoreOptions> options, AppStoreHookInterceptor hookInterceptor)
        {
            var pathMatched = context.Request.Path.StartsWithSegments(options.Value.HookInterceptorUri.AbsolutePath);
            var isPost = context.Request.Method.Equals("POST", caseSensitive: false);

            if (pathMatched && isPost)
            {
                await hookInterceptor.Intercept(await context.Request.Body.ReadAllText());
                await context.Response.WriteAsync($"{hookInterceptor.GetType().Name} executed.");
                return;
            }

            await next(context);
        }
    }
}
