using CodeName.EventSystem.Tasks;

namespace CodeName.EventSystem.GameEvents
{
    public interface IGameEventHandler<TGameState>
    {
        public StateTask OnEventRaised(IGameStateTracker<TGameState> tracker);

        public StateTask OnEventConfirmed(IGameStateTracker<TGameState> tracker);

        public StateTask OnEventApplied(IGameStateTracker<TGameState> tracker);
    }
}
