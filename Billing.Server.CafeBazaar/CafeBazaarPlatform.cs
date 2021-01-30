namespace Zebble.Billing
{
    abstract class CafeBazaarPlatform : IPlatformAware
    {
        public SubscriptionPlatform Platform => SubscriptionPlatform.CafeBazaar;
    }
}
