using CodeName.Serialization.Validation;
using Newtonsoft.Json;

namespace CodeName.EventEngine.GameEvents
{
    /// <summary>
    /// Null object used to replace an event that was prevented.
    /// </summary>
    public class PreventedEvent<TState> : GameEvent<TState>
    {
        public PreventedEvent(GameEvent<TState> originalEvent)
        {
            OriginalEvent = originalEvent;
        }

        [JsonProperty(TypeNameHandling = TypeNameHandling.Auto)] [SerializeByValue]
        public GameEvent<TState> OriginalEvent { get; }

        public override string ToString()
        {
            return $"{GetType().Name} - {OriginalEvent}";
        }
    }
}
