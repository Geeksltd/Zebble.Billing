namespace Zebble.Billing
{
    public class HuaweiOptions : StoreOptionsBase
    {
        public string PublicKey { get; set; }
        public string NotificationInterceptorPath { get; set; } = "huawei/intercept-notification";
    }
}
