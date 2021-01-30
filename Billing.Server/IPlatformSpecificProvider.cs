namespace Zebble.Billing
{
    using System.Collections.Generic;

    public interface IPlatformSpecificProvider<TType> : IEnumerable<TType> where TType : IPlatformAware
    {
        TType this[SubscriptionPlatform platform] { get; }
    }
}
