namespace Zebble.Billing
{
	public class GooglePlayOptions
	{
		public GooglePlayStoreOptions Store { get; set; }
		public GooglePlayPubSubOptions PubSub { get; set; }
		public string QueueProcessorPath { get; set; } = "google-play/process-queue";
	}
}
