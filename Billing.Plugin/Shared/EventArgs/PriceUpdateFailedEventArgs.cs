namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;

    public class PriceUpdateFailedEventArgs : EventArgs
    {
        public string[] ProductIds { get; internal set; }
        public Func<string, decimal, string, Task> UpdatePrice { get; internal set; }
    }
}
