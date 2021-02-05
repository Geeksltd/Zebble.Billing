namespace Zebble.Billing.Sample
{
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    [ApiController]
    [Route("hooks")]
    public class HooksController : ControllerBase
    {
        readonly IRootHookInterceptor rootHookInterceptor;

        public HooksController(IRootHookInterceptor rootHookInterceptor)
        {
            this.rootHookInterceptor = rootHookInterceptor;
        }

        [HttpPost("intercept/{platform}")]
        public async Task<string> Intercept([FromRoute] string platform, [FromBody] string body)
        {
            await rootHookInterceptor.Intercept(platform, body);

            return $"Trigerred hook intercepted. ({platform})";
        }
    }
}
