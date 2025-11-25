namespace Zebble.Billing
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Olive;

    class AppPurchaseAttemptsMiddleware
    {
        public AppPurchaseAttemptsMiddleware(RequestDelegate _) { }

        public async Task InvokeAsync(
            HttpContext context,
            ISubscriptionManager manager,
            ITicketValidator ticketValidator)
        {
            var model = await context.Request.Body.ConvertTo<Model>();

            if (await ticketValidator.IsValid(model.UserId, model.Ticket) == false)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Request is invalid.");
                return;
            }

            var results = await model.Purchases.Select(
                purchase => manager.PurchaseAttempt(
                    model.UserId,
                    purchase.Platform,
                    purchase.ProductId,
                    purchase.SubscriptionId,
                    purchase.TransactionId,
                    purchase.PurchaseToken,
                    model.ReplaceConfirmed
                )
            ).AwaitAll();

            await context.Response.WriteAsJsonAsync(new { Results = results });
        }
    }

    record Model(string UserId, string Ticket, Purchase[] Purchases, bool ReplaceConfirmed);

    record Purchase(string Platform, string ProductId, string SubscriptionId, string TransactionId, string PurchaseToken);

}
