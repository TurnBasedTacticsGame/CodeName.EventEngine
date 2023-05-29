using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Project.Source.Serialization;

namespace CodeName.EventSystem.State.Serialization
{
    public class EntityInstanceConverter<T> : JsonConverter where T : EntityAsset
    {
        private readonly string idPropertyName = nameof(EntityInstance.Id);
        private readonly string dataPropertyName = nameof(EntityInstance.Data);
        private readonly string assetPropertyName = nameof(EntityInstance<T>.Asset);

        private readonly ProjectJsonSerializer serializer;

        public EntityInstanceConverter(ProjectJsonSerializer serializer)
        {
            this.serializer = serializer;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer _)
        {
            var typedValue = (EntityInstance<T>)value;
            if (typedValue == null)
            {
                serializer.Serialize(writer, null);

                return;
            }

            var jsonObject = new JObject();
            jsonObject[idPropertyName] = JToken.FromObject(typedValue.Id, serializer);
            jsonObject[dataPropertyName] = JToken.FromObject(typedValue.Data, serializer);
            jsonObject[assetPropertyName] = JToken.FromObject(typedValue.UntypedAsset, serializer);

            jsonObject.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer _)
        {
            var jsonObject = JObject.Load(reader);
            var asset = serializer.Deserialize<T>(jsonObject[assetPropertyName]!);

            var idToken = jsonObject[idPropertyName];
            var result = new EntityInstance<T>(asset, () => idToken != null ? serializer.Deserialize<EntityId>(idToken) : EntityId.InvalidId);

            var dataToken = jsonObject[dataPropertyName];
            if (dataToken != null)
            {
                serializer.PopulateWithConverter(dataToken, result.Data);
            }

            return result;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(EntityInstance<T>);
        }
    }
}
