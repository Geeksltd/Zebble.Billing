namespace Zebble.Billing
{
    public interface IPlatformAware
    {
        SubscriptionPlatform Platform { get; }
    }
}
