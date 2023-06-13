namespace Zebble.Billing
{
    using System;
    using System.IO;
    using Huawei.Agconnect.Config;
    using Olive;

    class HmsLazyInputStream : LazyInputStream
    {
        public HmsLazyInputStream(Android.Content.Context context) : base(context) { }

        public override Stream Get(Android.Content.Context context)
        {
            try { return context.Assets.Open("agconnect-services.json"); }
            catch (Exception ex)
            {
                Log.For<HmsLazyInputStream>().Error(ex, $"Failed to get input stream" + ex.Message);
                return null;
            }
        }
    }
}
