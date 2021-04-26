namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    class VoucherCodeApplyingMiddleware
    {
        public VoucherCodeApplyingMiddleware(RequestDelegate _) { }

        public async Task InvokeAsync(HttpContext context, IVoucherManager voucherManager)
        {
            var (userId, code) = ParseRouteValues(context.Request.Path);

            await voucherManager.Apply(userId, code);
            await context.Response.WriteAsync("Voucher is applied.");
        }

        static (string, string) ParseRouteValues(PathString path)
        {
            if (path == null) throw new Exception("No param is provided.");

            var segments = path.Value.Split('/');

            // First segment is always empty
            if (segments.Length != 3) throw new Exception("Provided params doesn't match.");

            return (segments[1], segments[2]);
        }
    }
}
