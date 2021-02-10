namespace Zebble.Billing
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    public class GooglePlayQueueProcessingMiddleware
    {
        public async Task InvokeAsync(HttpContext context, GooglePlayQueueProcessor queueProcessor)
        {
            var processedMessages = await queueProcessor.Process();
            var responseText = processedMessages == 0 ? "No message found to process." : $"{processedMessages} messages are processed.";

            await context.Response.WriteAsync(responseText);
        }
    }
}
