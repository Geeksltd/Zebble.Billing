namespace Zebble.Plugin.Renderer
{
    using System;
    using System.ComponentModel;
    using Zebble;
    using System.Threading.Tasks;
    using TheNativeType = System.Object; //TODO: replace with the native Android type;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public class BillingRenderer : INativeRenderer
    {
        Billing View;
        TheNativeType Result;

        public async Task<Android.Views.View> Render(Renderer renderer)
        {
            View = (Billing)renderer.View; ;
            Result = new TheNativeType();

            // TODO: Map the methods to the native component:
            View.MyMethod1Implementation = () => Result.EquivalentForMethod1();
            View.MyMethod2Implementation = p1 => Result.EquivalentForMethod2(p1);
            // ...

            // TODO: Map the native component's events to the Zebble abstraction:
            Result.EquivalentNativeEvent1 += (s, e) => View.MyEvent1.Raise();
            Result.EquivalentNativeEvent2 += (s, e) => View.MyEvent2.Raise(e.UsefulArg);
            // ...

            // TODO: Map the properties to the native component:
            ApplyMyProperty();
            View.MyPropertyChanged.HandleActionOn(Thread.UI, ApplyMyProperty);
            // ...

            return Result;
        }

        void ApplyMyProperty()
        {
            // TODO: Feel free to transform the value if needed.
            // This method can return a Task if required.
            Result.MyPropertyEquivalent = View.MyProperty;
        }

        public void Dispose() => Result.Dispose();
    }
}