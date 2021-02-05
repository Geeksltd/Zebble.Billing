namespace Zebble.Billing
{
    using Microsoft.AspNetCore.Builder;

    public static class ZebbleBillingAppBuilderExtensions
    {
        public static ZebbleBillingAppBuilder UseAppStore(this ZebbleBillingAppBuilder builder)
        {
            builder.App.UseMiddleware<HookInterceptionMiddleware>();

            return builder;
        }
    }
}
