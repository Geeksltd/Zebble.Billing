namespace Zebble.Billing
{
    using Amazon.DynamoDBv2;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static partial class ZebbleBillingServicesBuilderExtensions
    {
        public static ZebbleBillingServicesBuilder AddDynamoDb(this ZebbleBillingServicesBuilder builder, string configKey = "ZebbleBilling:DynamoDb")
        {
            builder.Services.AddScoped(sp => sp.GetRequiredService<IConfiguration>().GetAWSOptions(configKey).CreateServiceClient<IAmazonDynamoDB>());

            builder.Services.AddScoped<SubscriptionDbContext>();

            builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();

            return builder;
        }
    }
}
