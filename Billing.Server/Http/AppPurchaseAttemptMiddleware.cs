namespace Zebble.Billing
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    class AppPurchaseAttemptMiddleware
    {
        public AppPurchaseAttemptMiddleware(RequestDelegate _) { }

        public async Task InvokeAsync(HttpContext context, ISubscriptionManager manager)
        {
            var model = await context.Request.Body.ConvertTo<AppPurchaseAttemptModel>();

            var result = await manager.PurchaseAttempt(
                model.UserId, model.Platform, model.ProductId, model.PurchaseToken, model.ReplaceConfirmed
            );

            await context.Response.WriteAsync(result.ToJson());
        }
    }

    class AppPurchaseAttemptModel
    {
        public string Ticket { get; set; }
        public string UserId { get; set; }

        public string Platform { get; set; }

        public string ProductId { get; set; }
        public string PurchaseToken { get; set; }

        public bool ReplaceConfirmed { get; set; }
    }
}
