namespace Zebble.Billing.Sample
{
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    [ApiController]
    [Route("app")]
    public class AppController : ControllerBase
    {
        readonly SubscriptionManager subscriptionManager;

        public AppController(SubscriptionManager subscriptionManager)
        {
            this.subscriptionManager = subscriptionManager;
        }

        [HttpPost("purchase-attempt")]
        public Task PurchaseAttempt([FromBody] AppPurchaseAttemptModel model)
        {
            return subscriptionManager.PurchaseAttempt(model.ProductId, model.UserId, model.Platform, model.PurchaseToken);
        }

        [HttpGet("subscription-status")]
        public Task<Subscription> SubscriptionStatus(string userId)
        {
            return subscriptionManager.GetSubscriptionStatus(userId);
        }
    }
}
