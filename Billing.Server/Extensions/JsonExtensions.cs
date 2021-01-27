namespace Zebble.Billing
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    static class JsonExtensions
    {
        public static string ToJson(this object value)
        {
            return JsonConvert.SerializeObject(value, CreateDefaultSettings());
        }

        public static T FromJson<T>(this string value)
        {
            return JsonConvert.DeserializeObject<T>(value, CreateDefaultSettings());
        }

        static JsonSerializerSettings CreateDefaultSettings()
        {
            return new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                },
                Formatting = Formatting.Indented
            };
        }
    }
}
