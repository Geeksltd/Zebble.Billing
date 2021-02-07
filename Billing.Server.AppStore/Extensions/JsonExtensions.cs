namespace Zebble.Billing
{
    static class JsonExtensions
    {
        public static AppStoreNotification ToNotification(this string body)
        {
            return body.FromJson<AppStoreNotification>().WithOriginalData(body);
        }
    }
}
