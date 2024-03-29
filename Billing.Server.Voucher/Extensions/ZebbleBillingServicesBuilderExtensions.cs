﻿namespace Zebble.Billing
{
    using System;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Olive;

    public static partial class ZebbleBillingServicesBuilderExtensions
    {
        public static ZebbleBillingServicesBuilder AddVoucher(
            this ZebbleBillingServicesBuilder builder,
            Action<ZebbleBillingVoucherServicesBuilder> voucherBuilder = null,
            string configKey = "ZebbleBilling:Voucher"
        )
        {
            builder.Services.AddOptions<VoucherOptions>()
                            .Configure<IConfiguration>((opts, config) => config.GetSection(configKey)?.Bind(opts))
                            .Validate(opts => opts.VoucherApplyPath.HasValue(), $"{nameof(VoucherOptions.VoucherApplyPath)} is empty.");

            builder.Services.AddStoreConnector<VoucherConnector>("Voucher");
            builder.Services.AddScoped<IVoucherManager, VoucherManager>();

            voucherBuilder?.Invoke(new ZebbleBillingVoucherServicesBuilder(builder.Services));

            return builder;
        }
    }
}
