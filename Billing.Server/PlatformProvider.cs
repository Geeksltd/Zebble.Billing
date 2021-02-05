namespace Zebble.Billing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.DependencyInjection;

    class PlatformProvider<TType> : IPlatformProvider<TType> where TType : IPlatformAware
    {
        readonly IServiceProvider serviceProvider;

        public PlatformProvider(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        IEnumerable<TType> All => serviceProvider.GetService<IEnumerable<TType>>();

        public bool IsSupported(string platform) => All.Any(x => x.Platform == platform);

        public TType this[string platform] => All.FirstOrDefault(x => x.Platform == platform) ?? throw new NotSupportedException($"{typeof(TType).Name} isn't supported in {platform}.");

        public IEnumerator<TType> GetEnumerator() => All.GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
