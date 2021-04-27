namespace Zebble.Billing
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    class AppVerifyPurchaseMiddleware
    {
        public AppVerifyPurchaseMiddleware(RequestDelegate _) { }

        public async Task InvokeAsync(HttpContext context, SubscriptionManager manager)
        {
            var model = await context.Request.Body.ConvertTo<AppVerifyPurchaseModel>();

            var result = await manager.VerifyPurchase(model.UserId, model.Platform, model.ProductId, model.TransactionId, model.ReceiptData);

            await context.Response.WriteAsync(result.ToJson());
        }
    }

    class AppVerifyPurchaseModel
    {
        public string Ticket { get; set; }
        public string UserId { get; set; }

        public string Platform { get; set; }

        public string ProductId { get; set; }
        public string TransactionId { get; set; }
        public string ReceiptData { get; set; }
    }
}
