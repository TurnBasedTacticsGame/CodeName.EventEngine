using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Project.Source.Serialization;

namespace CodeName.EventSystem.Serialization
{
    public class InstanceDataCollectionConverter : JsonConverter
    {
        private readonly ProjectJsonSerializer serializer;

        public InstanceDataCollectionConverter(ProjectJsonSerializer serializer)
        {
            this.serializer = serializer;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer _)
        {
            var typedValue = (InstanceDataCollection)value;

            writer.WriteStartObject();
            {
                foreach (var (_, data) in typedValue.Entries)
                {
                    writer.WritePropertyName(data.Key);
                    writer.WriteRawValue(serializer.Serialize(data));
                }
            }
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer _)
        {
            if (existingValue == null)
            {
                throw new ArgumentNullException(nameof(existingValue), $"{GetType().Name} can only serialize into existing {typeof(InstanceDataCollection)}s");
            }

            var typedValue = (InstanceDataCollection)existingValue;
            var jsonObject = JObject.Load(reader);

            foreach (var (_, data) in typedValue.Entries)
            {
                var json = jsonObject[data.Key];
                if (json == null)
                {
                    continue;
                }

                serializer.PopulateWithConverter(json, data);
            }

            return typedValue;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(InstanceDataCollection);
        }
    }
}
