namespace Zebble.Billing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Olive;

    class EnumConverter<T> : JsonConverter<T> where T : struct
    {
        static IDictionary<T, string> Cache;

        static void EnsureMembersDiscovered()
        {
            if (Cache != null) return;

            Cache = Enum.GetValues(typeof(T)).OfType<T>().ToDictionary(
                x => x,
                x => FindAttribute<EnumMemberAttribute>(x)?.Value
            );
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            EnsureMembersDiscovered();

            var value = reader.GetString();
            return Cache.SingleOrDefault(x => x.Value == value).Key;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            EnsureMembersDiscovered();

            writer.WriteStringValue(Cache[value]);
        }

        static TAttr FindAttribute<TAttr>(T value) where TAttr : Attribute
        {
            return typeof(T).GetMember(value.ToString())[0].GetCustomAttribute<TAttr>();
        }
    }
}
