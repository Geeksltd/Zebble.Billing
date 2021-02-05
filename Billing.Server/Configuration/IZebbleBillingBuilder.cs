namespace Zebble.Billing
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public interface IZebbleBillingBuilder
    {
        public IServiceCollection Services { get; }
        public IConfiguration Configuration { get; }
    }
}
