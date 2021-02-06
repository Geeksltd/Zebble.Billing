namespace Zebble.Billing
{
    using System;

    public class GooglePlayOptions : StoreOptionsBase
    {
        public Uri QueueProcessorUri { get; set; }

        internal new bool Validate()
        {
            if (QueueProcessorUri == null) throw new ArgumentNullException(nameof(QueueProcessorUri));
            if (QueueProcessorUri.IsAbsoluteUri == false) throw new InvalidOperationException($"{nameof(QueueProcessorUri)} should be absolute.");

            return base.Validate();
        }
    }
}
