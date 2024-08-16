namespace Zebble.Billing;

using System;
using System.Linq;
using System.Threading.Tasks;
using AppStoreServerApi.Models;
using Microsoft.Extensions.Logging;
using Olive;
using System.Collections.Generic;
using AppStoreServerApi;

class AppStoreConnector : IStoreConnector
{
    readonly ILogger<AppStoreConnector> Logger;
    readonly AppStoreOptions Settings;
    readonly AppStoreClient Client;

    public AppStoreConnector(
        ILogger<AppStoreConnector> logger,
        AppStoreClient client
    )
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        Client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<SubscriptionInfo> GetSubscriptionInfo(SubscriptionInfoArgs args)
    {
        var result = await GetVerifiedResult(args.OriginalTransactionId);
        if (result.None()) return null;

        return CreateSubscription(result);
    }

    SubscriptionInfo CreateSubscription(JWSTransactionDecodedPayload[] transactions)
    {
        var transaction = transactions.OrderBy(x => x.PurchaseDate).LastOrDefault();
        if (transaction is null)
        {
            Logger.LogWarning("The receipt contains no transaction info.");
            return null;
        }

        return new SubscriptionInfo
        {
            ProductId = transaction.ProductId,
            TransactionId = transaction.OriginalTransactionId,
            SubscriptionDate = transaction.PurchaseDate,
            ExpirationDate = transaction.ExpiresDate,
            CancellationDate = transaction.RevocationDate,
            AutoRenews = transaction.Type == InAppPurchaseProductType.AutoRenewableSubscription
        };
    }

    async Task<JWSTransactionDecodedPayload[]> GetVerifiedResult(string originalTransactionId)
    {
        var result = new List<JWSTransactionDecodedPayload>();

        string revision = null;

        while (true)
        {
            var history = await Client.GetTransactionHistoryAsync(originalTransactionId, revision);

            result.AddRange(history.SignedTransactions.Select(x => x.DecodedPayload));

            if (history.HasMore == false) break;
            revision = history.Revision;
        }

        return [.. result];
    }
}
