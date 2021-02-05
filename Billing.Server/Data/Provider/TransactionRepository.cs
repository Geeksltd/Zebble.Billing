namespace Zebble.Billing
{
    using System.Threading.Tasks;

    class TransactionRepository : ITransactionRepository
    {
        readonly BillingDbContext context;

        public TransactionRepository(BillingDbContext context) => this.context = context;

        public async Task<Transaction> Save(Transaction transaction)
        {
            await context.Transactions.AddAsync(transaction);
            await context.SaveChangesAsync();

            return transaction;
        }
    }
}
