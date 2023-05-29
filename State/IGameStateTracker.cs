using CodeName.EventSystem.State.GameEvents;
using CodeName.EventSystem.State.Tasks;

namespace CodeName.EventSystem.State
{
    public interface IGameStateTracker<TGameState>
    {
        public GameEventTracker<TGameState> Events { get; }
        public TGameState State { get; }

        public StateTask RaiseEvent(GameEvent<TGameState> gameEvent);
    }
}
