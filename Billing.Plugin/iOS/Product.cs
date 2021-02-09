namespace Zebble.Billing
{
    using Plugin.InAppBilling;

    partial class Product
    {
        public string LocalPrice => $"${Price}";
    }
}