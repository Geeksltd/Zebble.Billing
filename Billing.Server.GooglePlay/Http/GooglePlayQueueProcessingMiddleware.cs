namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    class GooglePlayQueueProcessingMiddleware(ILogger<GooglePlayQueueProcessingMiddleware> logger, RequestDelegate _)
    {
        public async Task InvokeAsync(HttpContext context, GooglePlayQueueProcessor processor)
        {
            try
            {
                var count = await processor.Process(context.RequestAborted);
                await context.Response.WriteAsync($"Processed: {count}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process Google Play queue.");
                await context.Response.WriteAsync($"Failed to process Google Play queue.: {ex.Message}");
            }
        }
    }
}
