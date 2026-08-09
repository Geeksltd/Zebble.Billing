namespace Zebble.Billing
{
	public class GooglePlayOptions
	{
		public GooglePlayStoreOptions Store { get; set; }
		public GooglePlayPubSubOptions PubSub { get; set; }
		public string QueueProcessorPath { get; set; } = "google-play/process-queue";
		public string NotificationProcessorPath { get; set; } = "google-play/process-notification";

		/// <summary>Max messages requested per Pub/Sub Pull.</summary>
		public int QueuePullBatchSize { get; set; } = 100;

		/// <summary>Max messages pulled in a single HTTP process-queue run.</summary>
		public int QueueMaxMessagesPerRun { get; set; } = 500;

		/// <summary>Max parallel notification handlers per pull batch.</summary>
		public int QueueMaxDegreeOfParallelism { get; set; } = 8;

		/// <summary>Timeout in seconds for each Pull RPC (empty queues return after this).</summary>
		public int QueuePullTimeoutSeconds { get; set; } = 5;
	}
}
