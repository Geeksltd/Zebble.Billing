# Zebble.Billing

This is a one-stop and end-to-end solution for in-app purchases and subscriptions for solutions consisting of Zebble mobile apps and Olive server apps.
To understand how it all works in different platforms, read [this introduction](https://medium.com/@jmn8718/in-app-purchases-notifications-4408c3ee88eb).

# How to install?

### Zebble app

In the Zebble mobile app, install the appropriate nuget packages on all your supported platforms:
- **[Zebble.Billing](https://www.nuget.org/packages/Zebble.Billing/)** add to all projects
- [Zebble.Billing.CafeBazaar](https://www.nuget.org/packages/Zebble.Billing.CafeBazaar/) add to Android if you need to support CafeBazaar

[![Zebble.Billing](https://img.shields.io/nuget/v/Zebble.Billing.svg?label=Zebble.Billing)](https://www.nuget.org/packages/Zebble.Billing/)
[![Zebble.Billing.CafeBazaar](https://img.shields.io/nuget/v/Zebble.Billing.CafeBazaar.svg?label=Zebble.Billing.CafeBazaar)](https://www.nuget.org/packages/Zebble.Billing.CafeBazaar/)

Then you should initialize the plugin in your app's StartUp class like below:

```c#
public partial class StartUp : Zebble.StartUp
{
    public override async Task Run()
    {
        // ...

        BillingContext.Initialize();

        // Try to fetch products' prices from the store.
        Thread.Pool.RunOnNewThread(BillingContext.Current.UpdateProductPrices);
        
        // Ask the server for the latest subscription status.
        Thread.Pool.RunOnNewThread(BillingContext.Current.BackgroundRefresh);
        
        // ...
    }
}
```

The `BillingContext.Initialize` call (without providing any argument), leads to the default options and you only need to add the following line to your `Config.xml` file.

```xml
<Billing.Base.Url value="http://<YOUR_SERVER_URL>" />
```

Then you need to create a JSON file named `Catalog.json` in the Resources directory and add all your `Subscription` and `InAppPurchase` products into it. Every app will have some products that the user can buy. Products are either one-off or they are recurring payments. Recurring products are called Subscriptions, and one-off products are called InApp-Products. Regardless, each product should be defined in the appropriate app store first (Google Play, Apple iTunes Connect, etc) with the appropriate settings. 

```json
{
  "Products": [
    {
      "Id": "my.app.subscription.yearly",
      "Platform": "",
      "Type": "Subscription",
      "Title": "My Yearly Test Subscription",
      "Months": 12,
      "Promo": "7 days free trial",
      "FreeDays": 7,
      "Price": 0
    }
  ]
}

```

#### Notes
The products need to be filled according to the products you've defined in your supporting stores, especially `Id`, and `Type`.
If any of your products are platform-specific, you need to fill `Platform` with one of the following values: [AppStore, GooglePlay, CafeBazaar, WindowsStore], otherwise leave it empty.
For InAppPurchase products, fill `Type` with `InAppPurchase`, otherwise use `Subscription`.

#### `BillingContextOptions` class

You can create an instance of `BillingContextOptions` and override the following properties:

`BaseUri`: Assigning a Uri to this prop will specify the base URL for any server invocations. Bear in mind, when you assign a value for this prop, you don't need to add `Billing.Base.Url` to your `Config.xml` file.

`VerifyPurchasePath`: The relative path to verify purchase endpoint. The default value is "app/verify-purchase".

`PurchaseAttemptPath`: The relative path to purchase attempt endpoint. The default value is "app/purchase-attempt".

`SubscriptionStatusPath`: The relative path to subscription status endpoint. The default value is "app/subscription-status".

`CatalogPath`: The path of the catalog file in the client app. The default value is "Catalog.json".

---

### Server side app

In the server-side ASP.NET Core app, install the following packages:

- **[Zebble.Billing.Server](https://www.nuget.org/packages/Zebble.Billing.Server/)** the base package used by all the following providers, so you don't need a direct reference to this package.
- **[Zebble.Billing.Server.GooglePlay](https://www.nuget.org/packages/Zebble.Billing.Server.GooglePlay/)** to support Google Play store.
- **[Zebble.Billing.Server.AppStore](https://www.nuget.org/packages/Zebble.Billing.Server.AppStore/)** to support Apple (iOS) via iTunes.
- [Zebble.Billing.Server.CafeBazaar](https://www.nuget.org/packages/Zebble.Billing.Server.CafeBazaar/) to support Cafe Bazaar store.
- [Zebble.Billing.Server.Voucher](https://www.nuget.org/packages/Zebble.Billing.Server.Voucher/) to support direct sales (outside of the app stores).

[![Zebble.Billing.Server](https://img.shields.io/nuget/v/Zebble.Billing.Server.svg?label=Zebble.Billing.Server)](https://www.nuget.org/packages/Zebble.Billing.Server/)
[![Zebble.Billing.Server.GooglePlay](https://img.shields.io/nuget/v/Zebble.Billing.Server.GooglePlay.svg?label=Zebble.Billing.Server.GooglePlay)](https://www.nuget.org/packages/Zebble.Billing.Server.GooglePlay/)
[![Zebble.Billing.Server.AppStore](https://img.shields.io/nuget/v/Zebble.Billing.Server.AppStore.svg?label=Zebble.Billing.Server.AppStore)](https://www.nuget.org/packages/Zebble.Billing.Server.AppStore/)
[![Zebble.Billing.Server.CafeBazaar](https://img.shields.io/nuget/v/Zebble.Billing.Server.CafeBazaar.svg?label=Zebble.Billing.Server.CafeBazaar)](https://www.nuget.org/packages/Zebble.Billing.Server.CafeBazaar/)
[![Zebble.Billing.Server.Voucher](https://img.shields.io/nuget/v/Zebble.Billing.Server.Voucher.svg?label=Zebble.Billing.Server.Voucher)](https://www.nuget.org/packages/Zebble.Billing.Server.Voucher/)

All the above providers need to collaborate with a data persistence implementation. At the moment, we're only supporting EntityFramework (Sql Server), but we'll add the support to other options soon. Also any contribution to add other persisting options is welcome. 

- **[Zebble.Billing.Server.EntityFramework](https://www.nuget.org/packages/Zebble.Billing.Server.EntityFramework/)** to use RDBMS for subscription management
- - [Zebble.Billing.Server.Voucher.EntityFramework](https://www.nuget.org/packages/Zebble.Billing.Server.Voucher.EntityFramework/) to use RDBMS for voucher management

[![Zebble.Billing.Server.EntityFramework](https://img.shields.io/nuget/v/Zebble.Billing.Server.EntityFramework.svg?label=Zebble.Billing.Server.EntityFramework)](https://www.nuget.org/packages/Zebble.Billing.Server.EntityFramework/)
[![Zebble.Billing.Server.Voucher.EntityFramework](https://img.shields.io/nuget/v/Zebble.Billing.Server.Voucher.EntityFramework.svg?label=Zebble.Billing.Server.Voucher.EntityFramework)](https://www.nuget.org/packages/Zebble.Billing.Server.Voucher.EntityFramework/)

Then add the required configuration and files from [this sample app](https://github.com/Geeksltd/Zebble.Billing/tree/master/Billing.Sample).

---


# How does it work?
[edit](https://app.diagrams.net/#HGeeksltd%2FZebble.Billing%2Fmaster%2FArchitecture.png)

![](https://github.com/Geeksltd/Zebble.Billing/raw/master/Architecture.png)

---
