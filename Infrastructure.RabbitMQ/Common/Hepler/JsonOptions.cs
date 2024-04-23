using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.RabbitMQ.Common.Extensions
{
    public static class JsonOptions
    {
        public static JsonSerializerOptions Default { get; } = new(JsonSerializerDefaults.Web)
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
        };
    }
}
