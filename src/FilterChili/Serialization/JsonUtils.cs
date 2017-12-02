using Newtonsoft.Json;

namespace GravityCTRL.FilterChili.Serialization
{
    public class JsonUtils
    {
        public static JsonSerializer Serializer { get; }

        static JsonUtils()
        {
            var settings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ContractResolver = new JsonContract()
            };
            Serializer = JsonSerializer.Create(settings);
        }
    }
}
