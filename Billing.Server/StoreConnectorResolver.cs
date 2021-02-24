namespace Zebble.Billing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Olive;

    class StoreConnectorResolver : IStoreConnectorResolver
    {
        readonly IServiceProvider ServiceProvider;
        readonly IEnumerable<StoreConnectorRegistry> StoreConnectorRegistries;

        public StoreConnectorResolver(IServiceProvider serviceProvider, IEnumerable<StoreConnectorRegistry> storeConnectorRegistries)
        {
            ServiceProvider = serviceProvider;
            StoreConnectorRegistries = storeConnectorRegistries ?? throw new ArgumentNullException(nameof(storeConnectorRegistries));
        }

        public IStoreConnector Resolve(string storeName)
        {
            var registries = StoreConnectorRegistries.Where(x => x.Name == storeName);

            if (registries == null) throw new NotSupportedException($"Couldn't find any registry with name '{storeName}'.");

            if (registries.HasMany()) throw new NotSupportedException($"Found multiple registries with name '{storeName}'.");

            return (IStoreConnector)ServiceProvider.GetService(registries.Single().Type);
        }
    }
}
