using CodeName.EventEngine.Tasks;

namespace CodeName.EventEngine.GameEvents
{
    public interface IGameEventHandler<TGameState>
    {
        public StateTask OnEventRaised(IGameStateTracker<TGameState> tracker);

        public StateTask OnEventConfirmed(IGameStateTracker<TGameState> tracker);

        public StateTask OnEventApplied(IGameStateTracker<TGameState> tracker);
    }
}
