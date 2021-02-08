namespace Zebble.Billing
{
    public interface IBillingUser
    {
        string Ticket { get; }
        string UserId { get; }
    }
}
