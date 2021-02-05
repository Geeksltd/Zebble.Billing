namespace Zebble.Billing
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.AspNetCore.Builder;
    using Olive;

    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseZebbleBilling(this IApplicationBuilder app, Action<IZebbleBillingConfigurator> configurator = null)
        {
            app.ApplicationServices.GetService<IEnumerable<IHookInterceptor>>()
                .Do(x => app.UseMiddleware<HookInterceptionMiddleware>(x));

            configurator?.Invoke(new ZebbleBillingConfigurator(app));

            return app;
        }
    }
}
