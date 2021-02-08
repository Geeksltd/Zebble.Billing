namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    class VoucherCodeApplyingMiddleware
    {
        public async Task InvokeAsync(HttpContext context, VoucherCodeApplier codeApplier)
        {
            var (userId, platform, code) = ParseRouteValues(context.Request.Path);

            await codeApplier.Apply(userId, platform, code);
            await context.Response.WriteAsync("Voucher is applied.");
        }

        static (string, string, string) ParseRouteValues(PathString path)
        {
            if (path == null) throw new Exception("No param is provided.");

            var segments = path.Value.Split('/');

            // First segment is always empty
            if (segments.Length != 4) throw new Exception("Provided params doesn't match.");

            return (segments[1], segments[2], segments[3]);
        }
    }
}
