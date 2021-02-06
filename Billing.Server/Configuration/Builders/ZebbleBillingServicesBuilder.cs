namespace Zebble.Billing
{
    using System;
    using Microsoft.Extensions.DependencyInjection;

    public class ZebbleBillingServicesBuilder
    {
        public IServiceCollection Services { get; private set; }

        public ZebbleBillingServicesBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }
    }
}
