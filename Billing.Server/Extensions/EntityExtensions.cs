namespace Zebble.Billing
{
    static partial class EntityExtensions
    {
        public static bool RequiresStoreUpdate(this Subscription @this) => @this.IsExpired() || @this.IsCanceled();
    }
}
