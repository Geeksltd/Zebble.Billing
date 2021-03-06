﻿namespace Zebble.Billing
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;

    class VoucherDbContext : DbContext
    {
        readonly EntityFrameworkOptions Options;

        public DbSet<Voucher> Vouchers { get; set; }

        public VoucherDbContext(IOptionsSnapshot<EntityFrameworkOptions> options)
        {
            Options = options.Value;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseSqlServer(Options.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Voucher>().ToTable("Vouchers", "Voucher");
        }
    }
}
