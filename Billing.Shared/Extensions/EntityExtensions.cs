namespace Zebble.Billing
{
    using Olive;

    public static partial class EntityExtensions
    {
        public static bool IsExpired(this Subscription @this) => @this.ExpirationDate?.IsInThePast() ?? false;

        public static bool IsCanceled(this Subscription @this) => @this.CancellationDate?.IsInThePast() ?? false;
    }
}
