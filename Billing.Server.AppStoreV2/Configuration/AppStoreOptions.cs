namespace Zebble.Billing;

class AppStoreOptions : StoreOptionsBase
{
    public string PrivateKey { get; set; }
    public string KeyId { get; set; }
    public string IssuerId { get; set; }
    public AppStoreEnvironment Environment { get; set; }
    public bool AllowEnvironmentMixing { get; set; }
    public string HookInterceptorPath { get; set; } = "app-store/intercept-hook/v2";
}
