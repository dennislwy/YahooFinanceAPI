using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Threading.Tasks;

namespace YahooFinanceAPI.Utils
{
    //credits to Ali Zahid
    internal static class Json
    {
        private static readonly JsonSerializerSettings SerializeToCamelCase = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
        private static readonly JsonSerializer Serializer = JsonSerializer.Create(SerializeToCamelCase);

        public static async Task<T> ToObjectAsync<T>(string value)
        {
            return await Task.Run(() => JsonConvert.DeserializeObject<T>(value, SerializeToCamelCase)).ConfigureAwait(false);
        }

        public static async Task<T> ToObjectAsync<T>(JsonReader reader)
        {
            return await Task.Run(() => Serializer.Deserialize<T>(reader)).ConfigureAwait(false);
        }

        public static async Task<T> ToObjectAsync<T>(JToken jToken)
        {
            return await Task.Run(() => jToken.ToObject<T>(Serializer)).ConfigureAwait(false);
        }

        public static async Task<string> StringifyAsync(object value)
        {
            return await Task.Run(() => JsonConvert.SerializeObject(value, SerializeToCamelCase)).ConfigureAwait(false);
        }

        public static async Task<string> StringifyAsync(object value, Formatting formatting)
        {
            return await Task.Run(() => JsonConvert.SerializeObject(value, formatting, SerializeToCamelCase)).ConfigureAwait(false);
        }
    }
}