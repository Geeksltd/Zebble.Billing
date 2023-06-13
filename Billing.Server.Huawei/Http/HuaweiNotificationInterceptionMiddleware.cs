namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    class HuaweiNotificationInterceptionMiddleware
    {
        readonly ILogger<HuaweiNotificationInterceptionMiddleware> Logger;

        public HuaweiNotificationInterceptionMiddleware(ILogger<HuaweiNotificationInterceptionMiddleware> logger, RequestDelegate _)
        {
            Logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, HuaweiNotificationInterceptor hookInterceptor)
        {
            var body = "(null)";

            try
            {
                body = await context.Request.Body.ReadAsString();

                var notification = body.FromJson<HuaweiNotification>().WithOriginalData(body);

                await hookInterceptor.Intercept(notification);
                Logger.LogDebug($"The following notification intercepted successfully. {body}");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Failed to intercept the following notification. {body}");
                throw;
            }
        }
    }
}
