namespace Zebble.Billing
{
    using System;
    using Olive;

    public class DbContextOptions
    {
        public string ConnectionString { get; set; }

        internal bool Validate()
        {
            if (ConnectionString.IsEmpty()) throw new ArgumentNullException(nameof(ConnectionString));

            return true;
        }
    }
}
