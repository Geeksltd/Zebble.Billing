namespace Zebble.Billing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.DependencyInjection;

    class PlatformProvider<TType> : IPlatformProvider<TType> where TType : IPlatformAware
    {
        private readonly IServiceProvider _serviceProvider;

        public PlatformProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        IEnumerable<TType> All => _serviceProvider.GetService<IEnumerable<TType>>();

        public TType this[SubscriptionPlatform platform] => All.FirstOrDefault(x => x.Platform == platform) ?? throw new NotSupportedException($"{typeof(TType).Name} isn't supported in {platform}.");

        public IEnumerator<TType> GetEnumerator() => All.GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
