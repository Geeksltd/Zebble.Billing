namespace Zebble.Billing.Sample
{
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    [ApiController]
    [Route("app")]
    public class AppController : ControllerBase
    {
        readonly ISubscriptionManager subscriptionManager;

        public AppController(ISubscriptionManager subscriptionManager)
        {
            this.subscriptionManager = subscriptionManager;
        }

        [HttpPost("initiate-purchase")]
        public Task InitiatePurchase([FromBody] AppInitiatePurchaseModel model)
        {
            return subscriptionManager.InitiatePurchase(model.ProductId, model.UserId, model.Platform, model.PurchaseToken);
        }

        [HttpGet("subscription-status")]
        public Task<Subscription> SubscriptionStatus(string userId)
        {
            return subscriptionManager.GetSubscriptionStatus(userId);
        }
    }
}
