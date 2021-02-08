namespace Zebble.Billing
{
    using System;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Olive;

    public static class ZebbleBillingAppBuilderExtensions
    {
        public static ZebbleBillingAppBuilder UseVoucher(this ZebbleBillingAppBuilder builder)
        {
            builder.App.MapWhen(MatchesCodeApplyEndpoint, builder => builder.UseMiddleware<VoucherCodeApplyingMiddleware>());

            return builder;
        }

        static bool MatchesCodeApplyEndpoint(HttpContext context)
        {
            var options = context.RequestServices.GetService<IOptionsSnapshot<VoucherOptions>>();

            if (!context.Request.Path.StartsWithSegments(options.Value.CodeApplyUri.AbsolutePath)) return false;

            if (!context.Request.Method.Equals("POST", caseSensitive: false)) return false;

            return true;
        }
    }
}
