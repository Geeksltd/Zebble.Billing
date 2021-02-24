namespace Zebble.Billing
{
    using System;
    using Microsoft.AspNetCore.Builder;

    public class ZebbleBillingVoucherAppBuilder
    {
        public IApplicationBuilder App { get; private set; }

        public ZebbleBillingVoucherAppBuilder(IApplicationBuilder app)
        {
            App = app ?? throw new ArgumentNullException(nameof(app));
        }
    }
}
