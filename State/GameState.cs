using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace CodeName.EventSystem.State
{
    public class GameState
    {
        [JsonProperty]
        private MetaData Meta { get; set; } = new();

        public virtual void InitializeDefaults() {}

        public EntityId GenerateId()
        {
            Meta.LastGeneratedId++;

            return new EntityId(Meta.LastGeneratedId);
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            InitializeDefaults();
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            InitializeDefaults();
        }

        private class MetaData
        {
            public int LastGeneratedId { get; set; }
        }
    }
}
