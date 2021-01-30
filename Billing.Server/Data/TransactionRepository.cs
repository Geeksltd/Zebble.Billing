namespace Zebble.Billing
{
    using System.Threading.Tasks;

    class TransactionRepository : ITransactionRepository
    {
        readonly BillingDbContext _context;

        public TransactionRepository(BillingDbContext context) => _context = context;

        public async Task<Transaction> Save(Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();

            return transaction;
        }
    }
}
