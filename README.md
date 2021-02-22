# Zebble.Billing

This is a one-stop and end-to-end solution for in-app purchases and subscriptions for solutions consisting of Zebble mobile apps and Olive server apps.
To understand how it all works in different platforms, read [this introduction](https://medium.com/@jmn8718/in-app-purchases-notifications-4408c3ee88eb).

# How to install?

### Zebble App

In the Zebble mobile app, add the appropriate nuget packages:
- **[Zebble.Billing](https://www.nuget.org/packages/Zebble.Billing/)** add to all projects
- [Zebble.Billing.CafeBazaar](https://www.nuget.org/packages/Zebble.Billing.CafeBazaar/) add to Android if you need to support CafeBazaar

In `Config.xml` add

```xml
???
```

---

### Server side app

In the server-side asp.net app, add the following:

- **[Zebble.Billing.Server.EntityFramework](https://www.nuget.org/packages/Zebble.Billing.Server.EntityFramework/)** to use RDBMS for subscription management
- **[Zebble.Billing.Server.GooglePlay](https://www.nuget.org/packages/Zebble.Billing.Server.GooglePlay/)** to support Google Play store
- **[Zebble.Billing.Server.AppStore](https://www.nuget.org/packages/Zebble.Billing.Server.AppStore/)** to support Apple (iOS) via iTunes
- [Zebble.Billing.Server.CafeBazaar](https://www.nuget.org/packages/Zebble.Billing.Server.CafeBazaar/) to support Cafe Bazaar store
- [Zebble.Billing.Server.Voucher.EntityFramework](https://www.nuget.org/packages/Zebble.Billing.Server.Voucher.EntityFramework/) if you want to support direct sales (outside of the app stores)

Then add the required configuration and files from [this sample app](https://github.com/Geeksltd/Zebble.Billing/tree/master/Billing.Sample).

---


# How does it work?
[edit](https://app.diagrams.net/#HGeeksltd%2FZebble.Billing%2Fmaster%2FArchitecture.png)

![](https://github.com/Geeksltd/Zebble.Billing/raw/master/Architecture.png)

---

### Products.json
Every app will have some products that the user can buy. Products are either one-off or they are recurring payments. Recurring products are called Subscriptions, and one-off products are called InApp-Products. Regardless, each product should be defined in the appropriate app store first (Google Play, Apple iTunes Connect, etc) with the appropriate settings. 

The product settings should also be added to the server side application in the `products.json` file as below.

```json
....
```

Also a copy of the same file should be added to the mobile app under `....?` folder.

