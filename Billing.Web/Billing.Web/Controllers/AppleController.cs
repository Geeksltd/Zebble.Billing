namespace Billing.Web
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Olive;
    using System;
    using System.Threading.Tasks;
    using Zebble;

    [ApiController]
    [Route("apple")]
    public class AppleController : ControllerBase
    {
        readonly string _applePassword;
        readonly ISubscriptionManager _subscriptionManager;

        public AppleController(IConfiguration configuration, ISubscriptionManager subscriptionManager)
        {
            _applePassword = configuration["AppleSharedKey"];
            _subscriptionManager = subscriptionManager;
        }

        [HttpPost("v2/notification")]
        public async Task<IActionResult> RecieveNotification([FromBody] AppleNotification notification)
        {
            Log.For(this).Debug(nameof(RecieveNotification), notification);

            try
            {
                if (notification.Password != _applePassword) return Unauthorized();

                Request.Body.Seek(0, System.IO.SeekOrigin.Begin);
                var body = await Request.Body.ReadAllText();

                var x = new 

                await _subscriptionManager.ProcessAppleNotification(body);
                // new AppStoreNotification(fullJson).Save();

                return Ok();
            }
            catch (Exception ex)
            {
                // await EmailService.SendError(ex, new[] { JsonConvert.SerializeObject(response) });
                return Ok();
            }
        }
    }
}
