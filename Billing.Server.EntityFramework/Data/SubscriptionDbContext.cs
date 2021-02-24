namespace Zebble.Billing
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;

    class SubscriptionDbContext : DbContext
    {
        readonly DbContextOptions Options;

        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        public SubscriptionDbContext(IOptionsSnapshot<DbContextOptions> options)
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

            modelBuilder.Entity<Subscription>().ToTable("Subscriptions", "Subscription");

            modelBuilder.Entity<Transaction>().ToTable("Transactions", "Subscription");
        }
    }
}
