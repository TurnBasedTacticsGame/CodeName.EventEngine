using CodeName.EventSystem.Tasks;

namespace CodeName.EventSystem.GameEvents
{
    public interface IGameEventHandler<TGameState>
    {
        public StateTask OnEventRaised(IGameStateTracker<TGameState> tracker, GameEventNode<TGameState> node);

        public StateTask OnEventConfirmed(IGameStateTracker<TGameState> tracker, GameEventNode<TGameState> node);

        public StateTask OnEventApplied(IGameStateTracker<TGameState> tracker, GameEventNode<TGameState> node);
    }
}
