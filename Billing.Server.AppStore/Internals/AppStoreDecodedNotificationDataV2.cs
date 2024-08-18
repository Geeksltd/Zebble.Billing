using System.Text.Json.Serialization;

namespace Zebble.Billing;

record AppStoreDecodedNotificationDataV2(
    string BundleId,
    string BundleVersion,
    AppStoreEnvironment Environment,
    [property: JsonConverter(typeof(AppStoreJWSTransactionDecodedPayloadV2Converter))] AppStoreJWSTransactionDecodedPayloadV2 SignedTransactionInfo,
    [property: JsonConverter(typeof(AppStoreJWSRenewalInfoDecodedPayloadV2Converter))] AppStoreJWSRenewalInfoDecodedPayloadV2? SignedRenewalInfo);