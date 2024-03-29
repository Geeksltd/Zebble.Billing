﻿namespace Zebble.Billing
{
    using Android.App;
    using Android.Content;
    using System;

    partial class BillingContext
    {
        public static void HandleActivityResult(int requestCode, Result resultCode, Intent data, IBillingUser user)
        {
            PurchaseSubscriptionCommand.HandlePurchaseResult(requestCode, data, user).RunInParallel();
        }

        public static string PaymentAuthority => "Huawei";
    }
}
