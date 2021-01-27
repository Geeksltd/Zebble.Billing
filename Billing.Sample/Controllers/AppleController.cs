//namespace Zebble.Billing.Sample
//{
//    using Microsoft.AspNetCore.Mvc;
//    using Microsoft.Extensions.Configuration;
//    using System.Threading.Tasks;
//    using Olive;

//    [ApiController]
//    [Route("apple")]
//    public class AppleController : ControllerBase
//    {
//        readonly string _applePassword;
//        readonly ISubscriptionManager _subscriptionManager;

//        public AppleController(IConfiguration configuration, ISubscriptionManager subscriptionManager)
//        {
//            _applePassword = configuration["AppleSharedKey"];
//            _subscriptionManager = subscriptionManager;
//        }

//        [HttpPost("v2/notification")]
//        public async Task<IActionResult> RecieveNotification([FromBody] AppleNotification notification)
//        {
//            if (notification.Password != _applePassword) return Unauthorized();

//            Request.Body.Seek(0, System.IO.SeekOrigin.Begin);
//            var body = await Request.Body.ReadAllText();

//            await _subscriptionManager.ProcessAppleNotification(body);
//            // new AppStoreNotification(fullJson).Save();

//            return Ok();
//        }
//    }
//}
