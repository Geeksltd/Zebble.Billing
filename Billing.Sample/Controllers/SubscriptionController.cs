//namespace Zebble.Billing.Sample
//{
//    using Microsoft.AspNetCore.Mvc;
//    using System.Threading.Tasks;
//    using Zebble;

//    [ApiController]
//    [Route("subscription")]
//    public class SubscriptionController : ControllerBase
//    {
//        readonly ISubscriptionManager _subscriptionManager;

//        public SubscriptionController(ISubscriptionManager subscriptionManager)
//        {
//            _subscriptionManager = subscriptionManager;
//        }

//        [HttpPost("refresh")]
//        public Task<Subscription[]> Refresh([FromBody] SubscriptionRefreshArgs args)
//        {
//            return _subscriptionManager.Refresh(args.UserId, args.Tokens);
//        }

//        public class SubscriptionRefreshArgs
//        {
//            public string UserId { get; set; }
//            public string[] Tokens { get; set; }
//        }
//    }
//}
