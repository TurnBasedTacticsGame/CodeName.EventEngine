using CodeName.Serialization.Validation;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace CodeName.EventEngine
{
    [ValidateSerializeByValue]
    public class EventLog<TState>
    {
        [JsonConstructor]
        private EventLog() {}

        /// <summary>
        /// Creates a new event log.
        /// </summary>
        /// <remarks>
        /// Parameters are not cloned.
        /// </remarks>
        public EventLog(GameEventNode<TState> events, TState originalState = default, TState expectedState = default)
        {
            OriginalState = originalState;
            ExpectedState = expectedState;

            Events = events;
        }

        /// <summary>
        /// The events that occurred.
        /// </summary>
        [NotNull]
        [SerializeByValue] public GameEventNode<TState> Events { get; set; }

        /// <summary>
        /// The original state prior to applying the <see cref="Events"/>.
        /// Applying the <see cref="Events"/> to the <see cref="OriginalState"/> should result in the <see cref="ExpectedState"/>, if it is available.
        /// </summary>
        /// <remarks>
        /// It is up to the user to decide whether to provide this value or not.
        /// </remarks>
        [CanBeNull]
        [SerializeByValue] public TState OriginalState { get; set; }

        /// <summary>
        /// The expected state after applying the <see cref="Events"/>.
        /// </summary>
        /// <remarks>
        /// It is up to the user to decide whether to provide this value or not.
        /// </remarks>
        [CanBeNull]
        [SerializeByValue] public TState ExpectedState { get; set; }

        /// <summary>
        /// Creates a new event log using the values of this log.
        /// </summary>
        /// <remarks>
        /// Values are not cloned.
        /// </remarks>
        public EventLog<TState> ShallowClone()
        {
            return new EventLog<TState>(Events, OriginalState, ExpectedState);
        }
    }
}
