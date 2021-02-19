namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    class GooglePlayQueueProcessingMiddleware
    {
        readonly RequestDelegate Next;

        public GooglePlayQueueProcessingMiddleware(RequestDelegate next)
        {
            Next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task InvokeAsync(HttpContext context, GooglePlayQueueProcessor queueProcessor)
        {
            var processedMessages = await queueProcessor.Process();
            var responseText = processedMessages == 0 ? "No message found to process." : $"{processedMessages} messages are processed.";

            await context.Response.WriteAsync(responseText);

            await Next(context);
        }
    }
}
