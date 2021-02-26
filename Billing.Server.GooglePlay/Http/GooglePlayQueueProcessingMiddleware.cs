namespace Zebble.Billing
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    class GooglePlayQueueProcessingMiddleware
    {
        public GooglePlayQueueProcessingMiddleware(RequestDelegate _) { }

        public async Task InvokeAsync(HttpContext context, GooglePlayQueueProcessor queueProcessor)
        {
            var processedMessages = await queueProcessor.Process();

            await context.Response.WriteAsync(processedMessages == 0 ? "No message found to process." : $"{processedMessages} messages are processed.");
        }
    }
}
