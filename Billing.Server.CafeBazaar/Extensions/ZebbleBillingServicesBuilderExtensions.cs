namespace Zebble.Billing
{
    using CafeBazaar.DeveloperApi;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class ZebbleBillingServicesBuilderExtensions
    {
        public static ZebbleBillingServicesBuilder AddCafeBazaar(this ZebbleBillingServicesBuilder builder)
        {
            builder.Services.AddCafeBazaarDeveloperApi(builder.Configuration, "CafeBazaar:DeveloperApi");

            builder.Services.Configure<CafeBazaarOptions>(opts => builder.Configuration.GetSection("CafeBazaar")?.Bind(opts));
            builder.Services.AddScoped<IStoreConnector, CafeBazaarConnector>();

            return builder;
        }
    }
}
