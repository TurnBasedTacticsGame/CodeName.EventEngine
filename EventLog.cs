using CodeName.Serialization.Validation;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace CodeName.EventSystem
{
    [ValidateSerializeByValue]
    public class EventLog<TGameState>
    {
        [JsonConstructor]
        private EventLog() {}

        public EventLog(GameEventNode<TGameState> events, [NotNull] TGameState originalState) : this(events, originalState, default) {}

        public EventLog(GameEventNode<TGameState> events, [NotNull] TGameState originalState, TGameState expectedState)
        {
            OriginalState = originalState;
            ExpectedState = expectedState;

            Events = events;
        }

        /// <summary>
        /// The original state. Applying the <see cref="Events"/> to the <see cref="OriginalState"/> should result in the <see cref="ExpectedState"/>, if it is available.
        /// </summary>
        [NotNull]
        [SerializeByValue] public TGameState OriginalState { get; set; }

        /// <summary>
        /// The expected state after applying the <see cref="Events"/>. Can be null.
        /// </summary>
        [CanBeNull]
        [SerializeByValue] public TGameState ExpectedState { get; set; }

        /// <summary>
        /// The events that occurred.
        /// </summary>
        [NotNull]
        [SerializeByValue] public GameEventNode<TGameState> Events { get; set; }
    }
}