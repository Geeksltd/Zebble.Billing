namespace Zebble.Billing
{
    using System.Text.Json;

    public static class JsonExtensions
    {
        public static string ToJson(this object value, JsonSerializerOptions options = null)
        {
            return JsonSerializer.Serialize(value, options);
        }

        public static T FromJson<T>(this string value, JsonSerializerOptions options = null)
        {
            return JsonSerializer.Deserialize<T>(value, options);
        }
    }
}
