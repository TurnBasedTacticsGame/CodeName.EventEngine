using CodeName.Serialization.Validation;
using Newtonsoft.Json;

namespace CodeName.EventEngine.GameEvents
{
    /// <summary>
    /// Null object used to replace an event that was prevented.
    /// </summary>
    public class PreventedEvent<TGameState> : GameEvent<TGameState>
    {
        public PreventedEvent(GameEvent<TGameState> originalEvent)
        {
            OriginalEvent = originalEvent;
        }

        [JsonProperty(TypeNameHandling = TypeNameHandling.Auto)] [SerializeByValue]
        public GameEvent<TGameState> OriginalEvent { get; }

        public override string ToString()
        {
            return $"{GetType().Name} - {OriginalEvent}";
        }
    }
}
