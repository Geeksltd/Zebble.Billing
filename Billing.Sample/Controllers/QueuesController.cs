namespace Zebble.Billing.Sample
{
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    [ApiController]
    [Route("queues")]
    public class QueuesController : ControllerBase
    {
        readonly IRootQueueProcessor _rootQueueProcessor;

        public QueuesController(IRootQueueProcessor rootQueueProcessor)
        {
            _rootQueueProcessor = rootQueueProcessor;
        }

        [HttpGet("process-all")]
        public async Task<string> ProcessAll()
        {
            var processedMessages = await _rootQueueProcessor.ProcessAll();

            if (processedMessages == 0)
                return "No message found to process.";

            return $"{processedMessages} messages are processed.";
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
