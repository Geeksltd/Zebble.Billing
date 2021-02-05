namespace Zebble.Billing
{
    using System;
    using Microsoft.AspNetCore.Builder;

    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseZebbleBilling(this IApplicationBuilder app, Action<ZebbleBillingAppBuilder> configurator = null)
        {
            configurator?.Invoke(new ZebbleBillingAppBuilder(app));

            return app;
        }
    }
}
