namespace Zebble.Billing
{
    static class JsonExtensions
    {
        public static AppStoreNotification ToNotification(this string body)
        {
            return null;

            //return body.FromJson<AppStoreNotification.UnderlayingType>().ToNotification(data);
        }
    }
}
