namespace Zebble.Billing
{
    using System.Collections.Generic;

    public interface IPlatformProvider<TType> : IEnumerable<TType> where TType : IPlatformAware
    {
        bool IsSupported(SubscriptionPlatform platform);
        TType this[SubscriptionPlatform platform] { get; }
    }
}
