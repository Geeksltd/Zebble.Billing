namespace Zebble.Billing
{
    using Google.Cloud.PubSub.V1;
    using Newtonsoft.Json;

    static class PubSubExtensions
    {
        public static GoogleNotification ToNotification(this PubsubMessage message)
        {
            var data = message.Data.ToStringUtf8();

            return JsonConvert.DeserializeObject<GoogleNotification.UnderlayingType>(data).ToNotification(data);
        }
    }
}
