namespace Zebble.Billing
{
    using System.Threading.Tasks;

    public interface ITicketValidator
    {
        Task<bool> IsValid(string userId, string ticket);
    }
}
