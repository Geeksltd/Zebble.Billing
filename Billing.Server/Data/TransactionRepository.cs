namespace Zebble.Billing
{
    using System.Threading.Tasks;

    public class TransactionRepository : ITransactionRepository
    {
        readonly BillingContext _context;

        public TransactionRepository(BillingContext context) => _context = context;

        public async Task<Transaction> Save(Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();

            return transaction;
        }
    }
}
