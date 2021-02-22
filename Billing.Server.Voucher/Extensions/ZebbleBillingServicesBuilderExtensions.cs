namespace Zebble.Billing
{
    using System;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static partial class ZebbleBillingServicesBuilderExtensions
    {
        public static ZebbleBillingServicesBuilder AddVoucher(this ZebbleBillingServicesBuilder builder, Action<ZebbleBillingVoucherServicesBuilder> voucherBuilder = null, string configKey = "ZebbleBilling:Voucher")
        {
            builder.Services.AddOptions<VoucherOptions>()
                            .Configure<IConfiguration>((opts, config) => config.GetSection(configKey)?.Bind(opts))
                            .PostConfigure<IHttpContextAccessor>((opts, contextAccessor) =>
                            {
                                if (opts.CodeApplyUri.IsAbsoluteUri) return;
                                opts.CodeApplyUri = contextAccessor.ToAbsolute(opts.CodeApplyUri);
                            })
                            .Validate(opts => opts.CodeApplyUri is not null, $"{nameof(VoucherOptions.CodeApplyUri)} is null.");

            builder.Services.AddStoreConnector<VoucherConnector>("Voucher");
            builder.Services.AddScoped<VoucherCodeApplier>();

            voucherBuilder?.Invoke(new ZebbleBillingVoucherServicesBuilder(builder.Services));

            return builder;
        }
    }
}
