namespace Zebble.Billing.Sample
{
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    [ApiController]
    [Route("queues")]
    public class QueuesController : ControllerBase
    {
        readonly IRootQueueProcessor rootQueueProcessor;

        public QueuesController(IRootQueueProcessor rootQueueProcessor)
        {
            this.rootQueueProcessor = rootQueueProcessor;
        }

        [HttpGet("process-all")]
        public async Task<string> ProcessAll()
        {
            var processedMessages = await rootQueueProcessor.ProcessAll();

            if (processedMessages == 0)
                return "No message found to process.";

            return $"{processedMessages} messages are processed.";
        }

        [HttpGet("process/{platform}")]
        public async Task<string> Process(string platform)
        {
            var processedMessages = await rootQueueProcessor.Process(platform);

            if (processedMessages == 0)
                return $"No message found to process. ({platform})";

            return $"{processedMessages} messages are processed. ({platform})";
        }
    }
}
