namespace Zebble.Billing
{
    using System;
    using Microsoft.AspNetCore.Builder;

    class ZebbleBillingConfigurator : IZebbleBillingConfigurator
    {
        public IApplicationBuilder App { get; private set; }

        public ZebbleBillingConfigurator(IApplicationBuilder app)
        {
            App = app ?? throw new ArgumentNullException(nameof(app));
        }
    }
}
