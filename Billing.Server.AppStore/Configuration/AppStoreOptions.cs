namespace Zebble.Billing
{
    using Apple.Receipt.Verificator.Models;

    public class AppStoreOptions : StoreOptionsBase
    {
        public string SharedSecret { get; set; }
        public string PrivateKey { get; set; }
        public string KeyId { get; set; }
        public string IssuerId { get; set; }
        public AppStoreEnvironment Environment { get; set; }
        public bool AllowEnvironmentMixing { get; set; }
        public string HookInterceptorPath { get; set; } = "app-store/intercept-hook";

        internal void Apply(AppleReceiptVerificationSettings @that)
        {
            @that.VerifyReceiptSharedSecret = SharedSecret;
            @that.VerificationType = Environment.ToVerificationType();
            @that.AllowedBundleIds = new string[] { null };
        }
    }
}
