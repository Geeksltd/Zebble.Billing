namespace Zebble.Billing.Sample
{
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using Olive;

    [ApiController]
    [Route("app")]
    public class AppController : ControllerBase
    {
        readonly SubscriptionManager SubscriptionManager;

        public AppController(SubscriptionManager subscriptionManager)
        {
            SubscriptionManager = subscriptionManager;
        }

        [HttpPost("purchase-attempt")]
        public async Task<IActionResult> PurchaseAttempt([FromBody] AppPurchaseAttemptModel model)
        {
            if (ValidateTicket(model.Ticket)) return Unauthorized();

            await SubscriptionManager.PurchaseAttempt(model.ProductId, model.UserId, model.Platform, model.PurchaseToken);

            return Ok();
        }

        [HttpPost("subscription-status/{ticket}/{userId}")]
        public async Task<IActionResult> SubscriptionStatus([FromBody] AppSubscriptionStatusModel model)
        {
            if (ValidateTicket(model.Ticket)) return Unauthorized();

            return Ok(await SubscriptionManager.GetSubscriptionStatus(model.UserId));
        }

        bool ValidateTicket(string ticket) => ticket.HasValue();
    }
}
