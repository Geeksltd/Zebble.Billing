namespace Zebble.Billing
{
    using System;

    public interface IBillingUser
    {
        string Ticket { get; }
        string UserId { get; }
        bool EverSubscribed { get; }
        string[] SubscriptionTokens { get; }
        DateTime? SubscriptionExpiry { get; }
        SubscriptionType? SubscriptionType { get; }
        string VoucherCode { get; }
    }
}
