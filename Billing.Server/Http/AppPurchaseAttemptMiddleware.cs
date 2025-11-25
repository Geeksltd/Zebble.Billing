namespace Zebble.Billing
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    class AppPurchaseAttemptMiddleware
    {
        public AppPurchaseAttemptMiddleware(RequestDelegate _) { }

        public async Task InvokeAsync(
            HttpContext context,
            ISubscriptionManager manager,
            ITicketValidator ticketValidator)
        {
            var model = await context.Request.Body.ConvertTo<AppPurchaseAttemptModel>();

            if (await ticketValidator.IsValid(model.UserId, model.Ticket) == false)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Request is invalid.");
                return;
            }

            var result = await manager.PurchaseAttempt(
                model.UserId,
                model.Platform,
                model.ProductId,
                model.SubscriptionId,
                model.TransactionId,
                model.PurchaseToken,
                model.ReplaceConfirmed
            );

            await context.Response.WriteAsJsonAsync(result);
        }
    }

    class AppPurchaseAttemptModel
    {
        public string UserId { get; set; }
        public string Ticket { get; set; }

        public string Platform { get; set; }

        public string ProductId { get; set; }
        public string SubscriptionId { get; set; }
        public string TransactionId { get; set; }
        public string PurchaseToken { get; set; }

        public bool ReplaceConfirmed { get; set; }
    }
}
