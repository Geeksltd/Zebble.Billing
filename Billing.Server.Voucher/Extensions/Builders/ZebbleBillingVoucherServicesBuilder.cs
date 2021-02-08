namespace Zebble.Billing
{
    using System;
    using Microsoft.Extensions.DependencyInjection;

    public class ZebbleBillingVoucherServicesBuilder
    {
        public IServiceCollection Services { get; private set; }

        public ZebbleBillingVoucherServicesBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }
    }
}
