namespace Zebble.Billing
{
    static partial class EntityExtensions
    {
        internal static bool RequiresStoreUpdate(this Subscription @this) => @this.IsExpired() || @this.IsCanceled();
    }
}
