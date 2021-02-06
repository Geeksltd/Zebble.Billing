namespace Zebble.Billing
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    class StoreConnectorResolver
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
            var registry = StoreConnectorRegistries.FirstOrDefault(x => x.Name == storeName);
            if (registry == null) throw new NotSupportedException($"Couldn't find a registry with name '{storeName}'.");

            return (IStoreConnector)ServiceProvider.GetService(registry.Type);
        }
    }
}
