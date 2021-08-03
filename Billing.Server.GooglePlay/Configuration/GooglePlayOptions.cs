namespace Zebble.Billing
{
	public class GooglePlayOptions : StoreOptionsBase
	{
		public GooglePlayStoreOptions Store { get; set; }
		public GooglePlayPubSubOptions PubSub { get; set; }
		public string QueueProcessorPath { get; set; } = "google-play/process-queue";
	}
}
