namespace Zebble.Billing
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

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

            if (!context.Request.Matches(options.Value.CodeApplyUri)) return false;

            if (!context.Request.IsPost()) return false;

            return true;
        }
    }
}
