namespace Zebble.Billing
{
    using System;
    using Olive;

    class StoreConnectorRegistry
    {
        public string Name { get; private set; }
        public Type Type { get; set; }

        public StoreConnectorRegistry(string name, Type type)
        {
            Name = name.IsEmpty() ? throw new ArgumentNullException(nameof(name)) : name;
            Type = type ?? throw new ArgumentNullException(nameof(type));
        }
    }
}
