namespace Zebble.Billing
{
    using System;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

    public static class ZebbleBillingAppBuilderExtensions
    {
        public static ZebbleBillingAppBuilder UseVoucher(this ZebbleBillingAppBuilder builder, Action<ZebbleBillingVoucherAppBuilder> voucherBuilder = null)
        {
            var routes = new RouteBuilder(builder.App);

            var options = builder.App.ApplicationServices.GetService<IOptions<VoucherOptions>>();

            routes.MapMiddlewarePost(options.Value.VoucherApplyPath, builder => builder.UseMiddleware<VoucherApplyingMiddleware>());

            builder.App.UseRouter(routes.Build());

            voucherBuilder?.Invoke(new ZebbleBillingVoucherAppBuilder(builder.App));

            return builder;
        }
    }
}
