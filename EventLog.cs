using CodeName.Serialization.Validation;
using JetBrains.Annotations;

namespace CodeName.EventSystem
{
    public class EventLog<TGameState>
    {
        public EventLog([NotNull] TGameState originalState, TGameState expectedState)
        {
            OriginalState = originalState;
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
        [SerializeByValue] public GameEventNode<TGameState> Events { get; set; }
    }
}
