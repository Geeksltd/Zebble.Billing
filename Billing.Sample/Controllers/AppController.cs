namespace Zebble.Billing.Sample
{
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

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
        public Task PurchaseAttempt([FromBody] AppPurchaseAttemptModel model)
        {
            return SubscriptionManager.PurchaseAttempt(model.ProductId, model.UserId, model.Platform, model.PurchaseToken);
        }

        [HttpGet("subscription-status")]
        public Task<Subscription> SubscriptionStatus(string userId)
        {
            return SubscriptionManager.GetSubscriptionStatus(userId);
        }
    }
}
