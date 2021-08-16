namespace Zebble.Billing
{
	using System;
	using System.Threading.Tasks;
	using Microsoft.AspNetCore.Http;
	using Microsoft.Extensions.Logging;

	class AppStoreHookInterceptionMiddleware
	{
		readonly ILogger<AppStoreHookInterceptionMiddleware> Logger;

		public AppStoreHookInterceptionMiddleware(ILogger<AppStoreHookInterceptionMiddleware> logger, RequestDelegate _)
		{
			Logger = logger;
		}

		public async Task InvokeAsync(HttpContext context, AppStoreHookInterceptor hookInterceptor)
		{
			var body = "(null)";

			try
			{
				body = await context.Request.Body.ReadAsString();

				var notification = body.FromJson<AppStoreNotification>();

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
