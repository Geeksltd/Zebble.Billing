namespace Zebble.Billing
{
    public class HuaweiOptions : StoreOptionsBase
    {
        public string PublicKey { get; set; }
        public HuaweiEnvironment Environment { get; set; }
        public bool AllowEnvironmentMixing { get; set; }
        public string NotificationInterceptorPath { get; set; } = "huawei/intercept-notification";
    }
}
