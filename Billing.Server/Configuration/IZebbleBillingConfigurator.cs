namespace Zebble.Billing
{
    using Microsoft.AspNetCore.Builder;

    public interface IZebbleBillingConfigurator
    {
        public IApplicationBuilder App { get; }
    }
}
