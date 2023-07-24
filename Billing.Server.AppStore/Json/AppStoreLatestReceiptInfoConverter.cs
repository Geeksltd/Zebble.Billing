namespace Zebble.Billing
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    class AppStoreLatestReceiptInfoConverter : JsonConverter<AppStoreLatestReceiptInfo[]>
    {
        public override AppStoreLatestReceiptInfo[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                var objectConverter = (JsonConverter<AppStoreLatestReceiptInfo>)options
                        .GetConverter(typeof(AppStoreLatestReceiptInfo));

                return new[] { objectConverter.Read(ref reader, typeof(AppStoreLatestReceiptInfo), options) };
            }

            var arrayConverter = (JsonConverter<AppStoreLatestReceiptInfo[]>)options
                    .GetConverter(typeof(AppStoreLatestReceiptInfo[]));

            return arrayConverter.Read(ref reader, typeof(AppStoreLatestReceiptInfo[]), options);
        }

        public override void Write(Utf8JsonWriter writer, AppStoreLatestReceiptInfo[] value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
