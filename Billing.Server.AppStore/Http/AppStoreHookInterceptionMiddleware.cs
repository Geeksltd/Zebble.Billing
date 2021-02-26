namespace Zebble.Billing
{
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Olive;

    class AppStoreHookInterceptionMiddleware
    {
        public AppStoreHookInterceptionMiddleware(RequestDelegate _) { }

        public async Task InvokeAsync(HttpContext context, AppStoreHookInterceptor hookInterceptor)
        {
            var body = await ReadAsync(context.Request.Body);

            await hookInterceptor.Intercept(body);
            await context.Response.WriteAsync($"{nameof(AppStoreHookInterceptor)} executed.");
        }

        async Task<string> ReadAsync(Stream stream)
        {
            await using var copy = new MemoryStream();
            await stream.CopyToAsync(copy);

            copy.Seek(0, SeekOrigin.Begin);

            return await copy.ReadAllText();
        }
    }
}
