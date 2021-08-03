namespace Zebble.Billing
{
	using System.Threading.Tasks;
	using Microsoft.AspNetCore.Http;

	class GooglePlayQueueProcessingMiddleware
	{
		public GooglePlayQueueProcessingMiddleware(RequestDelegate _) { }

		public async Task InvokeAsync(HttpContext context, GooglePlayQueueProcessor queueProcessor)
		{
			var count = await queueProcessor.Process();
			await context.Response.WriteAsync($"Processed: {count}");
		}
	}
}
