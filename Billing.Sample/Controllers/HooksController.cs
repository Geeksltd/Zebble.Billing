namespace Zebble.Billing.Sample
{
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    [ApiController]
    [Route("hooks")]
    public class HooksController : ControllerBase
    {
        readonly IRootQueueProcessor _rootQueueProcessor;

        public HooksController(IRootQueueProcessor rootQueueProcessor)
        {
            _rootQueueProcessor = rootQueueProcessor;
        }

        [HttpGet("process/{platform}")]
        public async Task<string> Process(SubscriptionPlatform platform)
        {
            var processedMessages = await _rootQueueProcessor.Process(platform);

            if (processedMessages == 0)
                return $"No message found to process. ({platform})";

            return $"{processedMessages} messages are processed. ({platform})";
        }
    }
}
