using System;
using Newtonsoft.Json;

namespace CodeName.EventSystem.Serialization
{
    public class EventIdConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, (Guid)(EventId)value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return new EventId(serializer.Deserialize<Guid?>(reader) ?? Guid.Empty);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(EventId);
        }
    }
}
