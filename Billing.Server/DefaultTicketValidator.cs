namespace Zebble.Billing
{
    using System.Threading.Tasks;
    using Olive;

    public class DefaultTicketValidator : ITicketValidator
    {
        public Task<bool> IsValid(string userId, string ticket)
        {
            if (userId.IsEmpty()) return Task.FromResult(false);
            if (ticket.IsEmpty()) return Task.FromResult(false);
            return Task.FromResult(true);
        }
    }
}
