namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    class GooglePlayNotificationProcessingMiddleware(ILogger<GooglePlayNotificationProcessingMiddleware> logger, RequestDelegate _)
    {
        public async Task InvokeAsync(HttpContext context, GooglePlayNotificationProcessor processor)
        {
            try
            {
                var body = await context.Request.Body.ReadAsString();

                var notification = body.FromJson<GooglePlayNotification.UnderlayingType>().ToNotification(body);

                await processor.Process(notification);

                await context.Response.WriteAsync("Processed notification.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process Google Play queue.");
                await context.Response.WriteAsync($"Failed to process Google Play queue.: {ex.Message}");
            }
        }
    }
}
