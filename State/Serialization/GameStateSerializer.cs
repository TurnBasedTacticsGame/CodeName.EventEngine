using System.IO;
using Newtonsoft.Json;
using Project.Source.Serialization;
using UniDi;

namespace CodeName.EventSystem.State.Serialization
{
    public class GameStateSerializer
    {
        private readonly ProjectJsonSerializer internalSerializer;

        public GameStateSerializer(DiContainer parentContainer)
        {
            var container = parentContainer.CreateSubContainer();

            container.Bind<JsonConverter>().To<GameEntityIdConverter>().AsCached();
            container.Bind<JsonConverter>().To<GameResourceConverter>().AsCached();
            container.Bind<JsonConverter>().To<InstanceDataCollectionConverter>().AsCached();

            internalSerializer = container.Resolve<ProjectJsonSerializer>();
        }

        public string Serialize(object value)
        {
            return internalSerializer.Serialize(value);
        }

        public T Deserialize<T>(string json)
        {
            return internalSerializer.Deserialize<T>(json);
        }

        public T Clone<T>(T value)
        {
            using (var stringReader = new StringReader(internalSerializer.Serialize(value)))
            using (var textReader = new JsonTextReader(stringReader))
            {
                return (T)internalSerializer.Deserialize(textReader, value.GetType());
            }
        }

        private void BindGameInstanceConverter<T>(DiContainer container) where T : EntityAsset
        {
            container.Bind<JsonConverter>()
                .FromMethod(context =>
                {
                    return new EntityInstanceConverter<T>(context.Container.Resolve<ProjectJsonSerializer>());
                })
                .AsCached();
        }
    }
}
