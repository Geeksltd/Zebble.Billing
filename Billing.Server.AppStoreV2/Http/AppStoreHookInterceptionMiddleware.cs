namespace Zebble.Billing;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

class AppStoreHookInterceptionMiddleware
{
    readonly ILogger<AppStoreHookInterceptionMiddleware> Logger;

    public AppStoreHookInterceptionMiddleware(ILogger<AppStoreHookInterceptionMiddleware> logger, RequestDelegate _)
    {
        Logger = logger;
    }

    public async Task InvokeAsync(
        HttpContext context,
        AppStoreHookInterceptor hookInterceptor)
    {
        var body = "(null)";

        try
        {
            context.Request.EnableBuffering();
            body = await context.Request.Body.ReadAsString();

            var encodedNotification = body.FromJson<AppStoreEncodedNotification>();
            var decodedNotification = encodedNotification.Decode().WithOriginalData(body);

            await hookInterceptor.Intercept(decodedNotification);
            Logger.LogDebug($"The following notification intercepted successfully. {body}");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Failed to intercept the following notification. {body}");
            throw;
        }
    }
}
