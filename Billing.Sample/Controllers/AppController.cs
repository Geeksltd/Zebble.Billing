namespace Zebble.Billing.Sample
{
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    [ApiController]
    [Route("app")]
    public class AppController : ControllerBase
    {
        readonly ISubscriptionManager _subscriptionManager;

        public AppController(ISubscriptionManager subscriptionManager)
        {
            _subscriptionManager = subscriptionManager;
        }

        [HttpPost("initiate-purchase")]
        public Task InitiatePurchase([FromBody] AppInitiatePurchaseModel model)
        {
            return _subscriptionManager.InitiatePurchase(model.ProductId, model.UserId, model.Platform, model.PurchaseToken);
        }

        [HttpPost("refresh-subscriptions")]
        public Task<Subscription> RefreshSubscriptions([FromBody] AppRefreshSubscriptionsModel model)
        {
            return _subscriptionManager.GetSubscriptionInfo(model.UserId);
        }
    }
}
