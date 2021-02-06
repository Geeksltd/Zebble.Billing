namespace Zebble.Billing
{
    using Google.Cloud.PubSub.V1;

    static class PubSubExtensions
    {
        public static GooglePlayNotification ToNotification(this PubsubMessage message)
        {
            var data = message.Data.ToStringUtf8();

            return data.FromJson<GooglePlayNotification.UnderlayingType>().ToNotification(data);
        }
    }
}
