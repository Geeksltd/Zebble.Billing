namespace Zebble.Billing
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    class GooglePlayQueueProcessingMiddleware
    {
        public GooglePlayQueueProcessingMiddleware(RequestDelegate _) { }

        public Task InvokeAsync(HttpContext _, GooglePlayQueueProcessor queueProcessor)
        {
            return queueProcessor.Process();
        }
    }
}
