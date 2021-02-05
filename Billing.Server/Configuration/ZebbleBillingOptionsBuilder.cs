namespace Zebble.Billing
{
    using System;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class ZebbleBillingOptionsBuilder
    {
        public IServiceCollection Services { get; private set; }
        public IConfiguration Configuration { get; private set; }

        public ZebbleBillingOptionsBuilder(IServiceCollection services, IConfiguration configuration)
        {
            Services = services;
            Configuration = configuration?.GetSection("ZebbleBilling") ?? throw new ArgumentNullException(nameof(configuration));
        }
    }
}
