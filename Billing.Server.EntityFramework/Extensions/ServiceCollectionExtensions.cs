namespace Zebble.Billing
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static partial class ServiceCollectionExtensions
    {
        public static IZebbleBillingBuilder AddEntityFramework(this IZebbleBillingBuilder builder)
        {
            builder.Services.Configure<DbContextOptions>(opts => builder.Configuration.GetSection("DbContext")?.Bind(opts));
            builder.Services.AddDbContext<BillingDbContext>();

            builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();

            return builder;
        }
    }
}
