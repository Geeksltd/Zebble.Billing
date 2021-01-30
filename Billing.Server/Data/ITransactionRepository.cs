namespace Zebble.Billing
{
    using System.Threading.Tasks;

    public interface ITransactionRepository
    {
        Task<Transaction> Save(Transaction transaction);
    }
}
