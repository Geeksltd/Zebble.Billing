namespace Zebble.Billing
{
    abstract class GooglePlayPlatform : IPlatformAware
    {
        public SubscriptionPlatform Platform => SubscriptionPlatform.GooglePlay;
    }
}
