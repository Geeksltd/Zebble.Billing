namespace Zebble.Billing
{
    using System;
    using Microsoft.AspNetCore.Builder;

    public class ZebbleBillingAppBuilder
    {
        public IApplicationBuilder App { get; private set; }

        public ZebbleBillingAppBuilder(IApplicationBuilder app)
        {
            App = app ?? throw new ArgumentNullException(nameof(app));
        }
    }
}
