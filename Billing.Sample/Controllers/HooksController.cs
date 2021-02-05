namespace Zebble.Billing.Sample
{
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    [ApiController]
    [Route("hooks")]
    public class HooksController : ControllerBase
    {
        readonly IRootHookInterceptor _rootHookInterceptor;

        public HooksController(IRootHookInterceptor rootHookInterceptor)
        {
            _rootHookInterceptor = rootHookInterceptor;
        }

        [HttpPost("intercept/{platform}")]
        public async Task<string> Intercept([FromRoute] SubscriptionPlatform platform, [FromBody] string body)
        {
            await _rootHookInterceptor.Intercept(platform, body);

            return $"Trigerred hook intercepted. ({platform})";
        }
    }
}
