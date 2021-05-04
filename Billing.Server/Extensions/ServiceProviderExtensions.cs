namespace Zebble.Billing
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Olive;

    public static class ServiceProviderExtensions
    {
        public static IStoreConnector GetStoreConnector(IServiceProvider provider, string name)
        {
            if (provider is null) throw new ArgumentNullException(nameof(provider));

            if (name.IsEmpty()) throw new ArgumentNullException(nameof(name));

            return provider.GetService<StoreConnectorResolver>().Resolve(name);
        }
    }
}
