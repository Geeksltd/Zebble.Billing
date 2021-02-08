namespace Zebble.Billing
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;

    class BillingDbContext : DbContext
    {
        readonly DbContextOptions Options;

        public DbSet<Voucher> Vouchers { get; set; }

        public BillingDbContext(IOptionsSnapshot<DbContextOptions> options)
        {
            Options = options.Value;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseSqlServer(Options.ConnectionString);
        }
    }
}
