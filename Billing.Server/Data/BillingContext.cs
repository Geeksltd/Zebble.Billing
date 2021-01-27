namespace Zebble.Billing
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;

    public class BillingContext : DbContext
    {
        readonly DbContextOptions _options;

        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        public BillingContext(IOptions<DbContextOptions> options)
        {
            _options = options.Value;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseSqlServer(_options.ConnectionString);
        }
    }
}
