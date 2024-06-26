using System;
using System.Collections.Generic;
using System.Text;
using CodeName.EventEngine.GameEvents;
using CodeName.Serialization;
using CodeName.Serialization.Validation;
using Newtonsoft.Json;

namespace CodeName.EventEngine
{
    [ValidateSerializeByValue]
    public class GameEventNode<TGameState>
    {
        [JsonConstructor]
        private GameEventNode() {}

        public GameEventNode(GameEvent<TGameState> gameEvent, IEnumerable<int> path, ISerializer serializer, EventId eventId = default)
        {
            Id = eventId.IsValid() ? eventId : EventId.Generate();
            OriginalEvent = serializer.Clone(gameEvent);
            Event = serializer.Clone(gameEvent);

            Path = new List<int>(path);
        }

        /// <summary>
        /// Unique ID of this event.
        /// </summary>
        [JsonProperty] public EventId Id { get; private set; }

        /// <summary>
        /// The original event as created by the event raiser.
        /// <para/>
        /// Modifying this event is not recommended.
        /// </summary>
        [JsonProperty(TypeNameHandling = TypeNameHandling.Auto)] [SerializeByValue]
        public GameEvent<TGameState> OriginalEvent { get; private set; }

        /// <summary>
        /// The event that will be applied after the OnEventConfirmed phase.
        /// <para/>
        /// Modifying this event is allowed.
        /// </summary>
        [JsonProperty(TypeNameHandling = TypeNameHandling.Auto)] [SerializeByValue]
        public GameEvent<TGameState> Event { get; private set; }

        /// <summary>
        /// The path of this <see cref="GameEventNode{TState}"/> in the event tree.
        /// </summary>
        [JsonProperty]
        public List<int> Path { get; private set; }

        /// <summary>
        /// The events raised by this event being applied or in response to this event.
        /// <para/>
        /// In other words, child events are events caused by this event.
        /// </summary>
        [JsonProperty] [SerializeByValue]
        public List<GameEventNode<TGameState>> Children { get; private set; } = new();

        /// <summary>
        /// A locked event can no longer be prevented.
        /// </summary>
        [JsonProperty]
        public bool IsLocked { get; private set; }

        /// <summary>
        /// Expected game state after the event is applied. Usually used for debugging purposes.
        /// </summary>
        [JsonIgnore]
        public TGameState ExpectedState { get; set; }

        /// <summary>
        /// Prevent an event from being applied.
        /// <para/>
        /// An event can only be prevented during the OnEventRaised phase.
        /// Additionally, an event can only be prevented once.
        /// </summary>
        public void Prevent()
        {
            if (IsLocked)
            {
                throw new InvalidOperationException("Event has been locked. Events can only be prevented during the OnEventRaised event.");
            }

            if (Event is PreventedEvent<TGameState>)
            {
                throw new InvalidOperationException("Event has already been prevented. Events can only be prevented once.");
            }

            Event = new PreventedEvent<TGameState>(Event);
        }

        /// <summary>
        /// Called by <see cref="ISimulation{TGameState}"/> after the OnEventRaised event.
        /// </summary>
        public void Lock()
        {
            IsLocked = true;
        }

        public override string ToString()
        {
            return ToString(0);
        }

        private string ToString(int level)
        {
            var indent = new string(' ', level * 4);

            var childEventTextBuilder = new StringBuilder();
            foreach (var node in Children)
            {
                childEventTextBuilder.Append(node.ToString(level + 1));
            }

            return $"{indent}{Event}\n{childEventTextBuilder}";
        }
    }
}
