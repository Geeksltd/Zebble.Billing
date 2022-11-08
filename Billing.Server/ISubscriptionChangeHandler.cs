namespace Zebble.Billing
{
    using System.Threading.Tasks;

    public interface ISubscriptionChangeHandler
    {
        Task Handle(Subscription subscription);
    }
}
