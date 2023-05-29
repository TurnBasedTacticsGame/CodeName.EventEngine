using CodeName.EventSystem.State.Tasks;

namespace CodeName.EventSystem.State.GameEvents
{
    public interface IGameEventHandler<TGameState>
    {
        public StateTask OnEventRaised(GameStateTracker<TGameState> tracker, GameEventNode<TGameState> node);

        public StateTask OnEventConfirmed(GameStateTracker<TGameState> tracker, GameEventNode<TGameState> node);

        public StateTask OnEventApplied(GameStateTracker<TGameState> tracker, GameEventNode<TGameState> node);
    }
}
