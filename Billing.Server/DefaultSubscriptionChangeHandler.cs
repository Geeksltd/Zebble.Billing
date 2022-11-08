namespace Zebble.Billing
{
    using System.Threading.Tasks;

    class DefaultSubscriptionChangeHandler : ISubscriptionChangeHandler
    {
        public Task Handle(Subscription subscription) => Task.CompletedTask;
    }
}
