namespace Zebble.Billing
{
    using System;
    using System.Globalization;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Olive;

    class DateTimeConverter : JsonConverter<DateTime?>
    {
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            Convert(reader.GetString());

        public static DateTime? Convert(string value) =>
            DateTime.UnixEpoch.AddMilliseconds(value.To<long>());

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(((DateTimeOffset?)value)?.ToUnixTimeMilliseconds().ToString(CultureInfo.InvariantCulture).Or(""));
        }
    }
}
