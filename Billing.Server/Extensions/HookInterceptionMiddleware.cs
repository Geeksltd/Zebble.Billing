namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Olive;

    class HookInterceptionMiddleware
    {
        readonly RequestDelegate next;
        readonly IHookInterceptor hookInterceptor;

        public HookInterceptionMiddleware(RequestDelegate next, IHookInterceptor hookInterceptor)
        {
            this.next = next ?? throw new ArgumentNullException(nameof(next));
            this.hookInterceptor = hookInterceptor ?? throw new ArgumentNullException(nameof(hookInterceptor));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var pathMatched = context.Request.Path.StartsWithSegments(hookInterceptor.RelativeUri.AbsolutePath);
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
