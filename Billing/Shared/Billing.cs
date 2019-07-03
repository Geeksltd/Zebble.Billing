namespace Zebble.Plugin
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Zebble;

    public partial class Billing : View, IRenderedBy<Renderer.BillingRenderer>
    {
        // ---------------------- TODO: PROPERTIES -------------------------        
        // Use the following pattern for each property.
        MyPropertyType myProperty; // Used to hold the value.
        internal readonly AsyncEvent MyPropertyChanged = new AsyncEvent(); // Used to cascade changes to the native object.

        public MyPropertyType MyProperty // The public api for your component
        {
            get => myProperty;
            set { if (myProperty != value) { myProperty = value; MyPropertyChanged.Raise(); } }
        }


        // ---------------------- TODO: METHODS -------------------------        
        // For each method that you need, provide an internal delegate, and a public method.
        // Then provide the delegate's implementation in each platform's Renderer class.
        internal Action MyMethod1Implementation;
        public void MyMethod1() => MyMethod1Implementation?.Invoke();

        internal Func<int, string> MyMethod2Implementation;
        public string MyMethod2(int param1) => MyMethod2Implementation?.Invoke(param1);



        // ---------------------- TODO: EVENTS -------------------------        
        // For each event that you need, provide an AsyncEvent object.
        public readonly AsyncEvent MyEvent1 = new AsyncEvent();
        public readonly AsyncEvent<TEventArg> MyEvent2 = new AsyncEvent<TEventArg>();
    }
}