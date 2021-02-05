namespace Zebble.Billing
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;

    class BillingDbContext : DbContext
    {
        readonly DbContextOptions options;

        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        public BillingDbContext(IOptionsSnapshot<DbContextOptions> options)
        {
            this.options = options.Value;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseSqlServer(options.ConnectionString);
        }
    }
}
