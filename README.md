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

The `BillingContext.Initialize` call (without providing any argument), leads to the default options and you need to add the following line to your `Config.xml` file.

```xml
<Billing.Base.Url value="http://<YOUR_SERVER_URL>" />
```

Then create a JSON file named `Catalog.json` in the Resources directory and set the following properties:

`Build Action`: Embedded resource
`Copy to Output Directory`: Copy if newer

Thereupon, add all your products into it. Products are either one-off or they are recurring payments. Recurring products are called Subscriptions, and one-off products are called InApp-Products. Regardless, each product should be defined in the appropriate app store first (Google Play, Apple iTunes Connect, etc) with the appropriate settings. 

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

To override defaults, you can create an instance of `BillingContextOptions` and pass it to the `BillingContext.Initialize`. Here are the list of the properties you can provide:

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
- [Zebble.Billing.Server.Voucher.EntityFramework](https://www.nuget.org/packages/Zebble.Billing.Server.Voucher.EntityFramework/) to use RDBMS for voucher management

[![Zebble.Billing.Server.EntityFramework](https://img.shields.io/nuget/v/Zebble.Billing.Server.EntityFramework.svg?label=Zebble.Billing.Server.EntityFramework)](https://www.nuget.org/packages/Zebble.Billing.Server.EntityFramework/)
[![Zebble.Billing.Server.Voucher.EntityFramework](https://img.shields.io/nuget/v/Zebble.Billing.Server.Voucher.EntityFramework.svg?label=Zebble.Billing.Server.Voucher.EntityFramework)](https://www.nuget.org/packages/Zebble.Billing.Server.Voucher.EntityFramework/)

Then add the required configuration and files from [this sample app](https://github.com/Geeksltd/Zebble.Billing/tree/master/Billing.Sample).

#### Configuration

This is the sample settings file we included in the project to clearly show you what you need to set your web app up and running. 

```json
{
  "ZebbleBilling": {
    "Catalog": {
      "Products": [
        {
          "Id": "my.app.subscription.yearly",
          "Platform": "",
          "Type": "Subscription",
          "Title": "My Yearly Test Subscription",
          "Months": 12,
          "Promo": "7 days free trial",
          "FreeDays": 7
        }
      ]
    },
    "DbContext": {
      "ConnectionString": "Database=Billing.Sample; Server=.; Integrated Security=SSPI; MultipleActiveResultSets=True;"
    },
    "AppStore": {
      "PackageName": "<ios.package.name>",
      "SharedSecret": "<APP_STORE_SHARED_SECRET>",
      "Environment": "<Sandbox | Production>",
      "HookInterceptorUri": "app-store/intercept-hook"
    },
    "GooglePlay": {
      "PackageName": "<play.package.name>",
      "QueueProcessorUri": "google-play/process-queue",
      "ProjectId": "<PROJECT_ID>",
      "PrivateKeyId": "<PRIVATE_KEY_ID>",
      "PrivateKey": "<PRIVATE_KEY>",
      "ClientEmail": "<CLIENT_EMAIL>",
      "ClientId": "<CLIENT_ID>",
      "SubscriptionId": "<SUBSCRIPTION_ID>"
    },
    "CafeBazaar": {
      "PackageName": "<bazaar.package.name>",
      "DeveloperApi": {
        "RedirectUri": "cafe-bazaar/auth-callback",
        "ClientId": "<CLIENT_ID>",
        "ClientSecret": "<CLIENT_SECRET>"
      }
    },
    "Voucher": {
      "CodeApplyUri": "voucher/apply"
    }
  }
}
```

`Catalog`: This is the sample as the `Catalog.json` file we've talked about it ealier.

`DbContext`: The connection string used in both `Zebble.Billing.Server.EntityFramework` and `Zebble.Billing.Server.Voucher.EntityFramework` packages.

`AppStore:PackageName`: Your iOS app package name.

`AppStore:SharedSecret`: Your App Store connect shared secret. Follow [this article](https://docs.revenuecat.com/docs/itunesconnect-app-specific-shared-secret) to learn how you can create a shared secret.

`AppStore:Environment`: Use `Sandbox` when you're test-flighting your app, otherwise use `Production`. We do not allow mixed receipt validation. So if you configure it for the production environment, and attempt to validate a sandbox-based receipt, the whole process will be rejected.

`AppStore:HookInterceptorUri`: The relative path to hook interceptor endpoint.

---


# How does it work?
[edit](https://app.diagrams.net/#HGeeksltd%2FZebble.Billing%2Fmaster%2FArchitecture.png)

![](https://github.com/Geeksltd/Zebble.Billing/raw/master/Architecture.png)

---
