namespace Zebble.Billing
{
    abstract class AppStorePlatform : IPlatformAware
    {
        public SubscriptionPlatform Platform => SubscriptionPlatform.AppStore;
    }
}
