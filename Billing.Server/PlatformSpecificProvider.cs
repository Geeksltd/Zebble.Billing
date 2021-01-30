namespace Zebble.Billing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.DependencyInjection;

    class PlatformSpecificProvider<TType> : IPlatformSpecificProvider<TType> where TType : IPlatformAware
    {
        private readonly IServiceProvider _serviceProvider;

        public PlatformSpecificProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        IEnumerable<TType> All => _serviceProvider.GetService<IEnumerable<TType>>();

        public TType this[SubscriptionPlatform platform] => All.FirstOrDefault(x => x.Platform == platform);

        public IEnumerator<TType> GetEnumerator() => All.GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
