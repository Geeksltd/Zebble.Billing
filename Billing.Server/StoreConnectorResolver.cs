namespace Zebble.Billing
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    class StoreConnectorResolver
    {
        readonly IServiceProvider serviceProvider;
        readonly IEnumerable<StoreConnectorRegistry> storeConnectorRegistries;

        public StoreConnectorResolver(IServiceProvider serviceProvider, IEnumerable<StoreConnectorRegistry> storeConnectorRegistries)
        {
            this.serviceProvider = serviceProvider;
            this.storeConnectorRegistries = storeConnectorRegistries ?? throw new ArgumentNullException(nameof(storeConnectorRegistries));
        }

        public IStoreConnector Resolve(string storeName)
        {
            var registry = storeConnectorRegistries.FirstOrDefault(x => x.Name == storeName);
            if (registry == null) throw new NotSupportedException($"Couldn't find a registry with name '{storeName}'.");

            return (IStoreConnector)serviceProvider.GetService(registry.Type);
        }
    }
}
