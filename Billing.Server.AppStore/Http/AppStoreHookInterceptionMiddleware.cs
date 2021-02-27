namespace Zebble.Billing
{
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    class AppStoreHookInterceptionMiddleware
    {
        public AppStoreHookInterceptionMiddleware(RequestDelegate _) { }

        public async Task InvokeAsync(HttpContext context, AppStoreHookInterceptor hookInterceptor)
        {
            using var streamReader = new StreamReader(context.Request.Body, Encoding.UTF8);
            var body = await streamReader.ReadToEndAsync();

            await hookInterceptor.Intercept(body.ToNotification());
            await context.Response.WriteAsync($"{nameof(AppStoreHookInterceptor)} executed.");
        }
    }
}
