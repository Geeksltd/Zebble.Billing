namespace Zebble.Billing
{
    using System.Collections.Generic;

    public interface IPlatformProvider<TType> : IEnumerable<TType> where TType : IPlatformAware
    {
        bool IsSupported(string platform);
        TType this[string platform] { get; }
    }
}
