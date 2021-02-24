namespace Zebble.Billing
{
    using System;
    using System.Linq;
    using Microsoft.Extensions.DependencyInjection;

    class StoreConnectorResolver : IStoreConnectorResolver
    {
        readonly IServiceProvider ServiceProvider;

        public StoreConnectorResolver(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public IStoreConnector Resolve(string storeName)
        {
            var registry = ServiceProvider.GetServices<StoreConnectorRegistry>().FirstOrDefault(x => x.Name == storeName);
            if (registry == null) throw new NotSupportedException($"Couldn't find a registry with name '{storeName}'.");

            return (IStoreConnector)ServiceProvider.GetService(registry.Type);
        }
    }
}
