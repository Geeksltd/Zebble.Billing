namespace Zebble.Billing
{
    using Microsoft.Extensions.DependencyInjection;

    public static partial class ZebbleBillingServicesBuilderExtensions
    {
        public static ZebbleBillingServicesBuilder AddComparer<T>(this ZebbleBillingServicesBuilder builder) where T : class, ISubscriptionComparer
        {
            builder.Services.AddTransient<ISubscriptionComparer, T>();

            return builder;
        }
    }
}
