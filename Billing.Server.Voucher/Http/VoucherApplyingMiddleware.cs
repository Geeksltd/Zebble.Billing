namespace Zebble.Billing
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    class VoucherApplyingMiddleware
    {
        public VoucherApplyingMiddleware(RequestDelegate _) { }

        public async Task InvokeAsync(HttpContext context, IVoucherManager voucherManager)
        {
            var model = await context.Request.Body.ConvertTo<VocuherApplyModel>();

            await voucherManager.Apply(model.UserId, model.Code);
        }
    }

    class VocuherApplyModel
    {
        public string Ticket { get; set; }
        public string UserId { get; set; }

        public string Code { get; set; }
    }
}
