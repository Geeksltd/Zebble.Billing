namespace Zebble.Billing
{
    using System;
    using Olive;

    public abstract class StoreOptionsBase
    {
        public string PackageName { get; set; }

        protected virtual bool Validate()
        {
            if (PackageName.IsEmpty()) throw new ArgumentNullException(nameof(PackageName));

            return true;
        }
    }
}
