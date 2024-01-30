namespace Zebble.Billing
{
	using System;
	using System.Threading.Tasks;
	using Microsoft.AspNetCore.Http;
	using Microsoft.Extensions.Logging;

	class GooglePlayQueueProcessingMiddleware
	{
		readonly ILogger<GooglePlayQueueProcessingMiddleware> Logger;

		public GooglePlayQueueProcessingMiddleware(ILogger<GooglePlayQueueProcessingMiddleware> logger, RequestDelegate _)
		{
			Logger = logger;
		}

		public async Task InvokeAsync(HttpContext context, GooglePlayQueueProcessor queueProcessor)
		{
			try
			{
				var count = await queueProcessor.Process();
				await context.Response.WriteAsync($"Processed: {count}");
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, "Failed to process Google Play queue.");
				await context.Response.WriteAsync($"Failed to process Google Play queue.: {ex.Message}");
			}
		}
	}
}
