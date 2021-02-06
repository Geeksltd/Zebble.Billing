namespace Zebble.Billing
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;

    class BillingDbContext : DbContext
    {
        readonly DbContextOptions Options;

        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

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
