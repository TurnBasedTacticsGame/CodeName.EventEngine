using System;
using Newtonsoft.Json;

namespace CodeName.EventSystem.State.Serialization
{
    public class GameEntityIdConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, (int)(EntityId)value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return new EntityId(serializer.Deserialize<int?>(reader) ?? 0);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(EntityId);
        }
    }
}
