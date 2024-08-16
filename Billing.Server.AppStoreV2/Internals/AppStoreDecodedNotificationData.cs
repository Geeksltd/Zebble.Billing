using System.Text.Json.Serialization;

namespace Zebble.Billing;

record AppStoreDecodedNotificationData(
    string BundleId,
    string BundleVersion,
    AppStoreEnvironment Environment,
    [property: JsonConverter(typeof(AppStoreJWSTransactionDecodedPayloadConverter))] AppStoreJWSTransactionDecodedPayload SignedTransactionInfo,
    [property: JsonConverter(typeof(AppStoreJWSRenewalInfoDecodedPayloadConverter))] AppStoreJWSRenewalInfoDecodedPayload? SignedRenewalInfo);