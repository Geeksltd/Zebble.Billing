﻿using System;

namespace Zebble.Billing;

record AppStoreJWSTransactionDecodedPayloadV2(
    DateTimeOffset OriginalPurchaseDate, DateTimeOffset PurchaseDate, DateTimeOffset ExpiresDate,
    string OriginalTransactionId, AppStoreOfferType? OfferType, string ProductId, string TransactionId,
    long Price, string Currency, string Storefront, string Environment, bool IsUpgraded, string StorefrontId);