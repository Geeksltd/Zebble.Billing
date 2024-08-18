namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    class AppStoreHookInterceptionMiddleware
    {
        readonly ILogger<AppStoreHookInterceptionMiddleware> Logger;

        public AppStoreHookInterceptionMiddleware(ILogger<AppStoreHookInterceptionMiddleware> logger, RequestDelegate _)
        {
            Logger = logger;
        }

        public async Task InvokeAsync(
            HttpContext context,
            AppStoreHookInterceptor hookInterceptor,
            AppStoreHookInterceptorV2 hookInterceptorV2)
        {
            var body = "(null)";

            try
            {
                context.Request.EnableBuffering();
                body = await context.Request.Body.ReadAsString();

                try
                {
                    await ProcessV2();
                    Logger.LogDebug($"The following notification intercepted successfully (V2). {body}");
                }
                catch
                {
                    Logger.LogDebug($"Failed to intercept the following notification (V2). {body}. Falling back to V1.");
                    await ProcessV1();
                    Logger.LogDebug($"The following notification intercepted successfully (V1). {body}");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Failed to intercept the following notification. {body}");
                throw;
            }

            async Task ProcessV2()
            {
                var encodedNotification = body.FromJson<AppStoreEncodedNotificationV2>();
                var decodedNotification = encodedNotification.Decode().WithOriginalData(body);

                await hookInterceptorV2.Intercept(decodedNotification);
            }

            async Task ProcessV1()
            {
                var notification = body.FromJson<AppStoreNotification>().WithOriginalData(body);

                await hookInterceptor.Intercept(notification);
            }
        }
    }
}
