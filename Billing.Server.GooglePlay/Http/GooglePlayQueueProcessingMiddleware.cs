namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Options;
    using Olive;

    class GooglePlayQueueProcessingMiddleware
    {
        readonly RequestDelegate next;

        public GooglePlayQueueProcessingMiddleware(RequestDelegate next)
        {
            this.next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task InvokeAsync(HttpContext context, IOptionsSnapshot<GooglePlayOptions> options, GooglePlayQueueProcessor queueProcessor)
        {
            var pathMatched = context.Request.Path.StartsWithSegments(options.Value.QueueProcessorUri.AbsolutePath);
            var isPost = context.Request.Method.Equals("POST", caseSensitive: false);

            if (pathMatched && isPost)
            {
                var processedMessages = await queueProcessor.Process();
                var responseText = processedMessages == 0 ? "No message found to process." : $"{processedMessages} messages are processed.";

                await context.Response.WriteAsync(responseText);
                return;
            }

            await next(context);
        }
    }
}
