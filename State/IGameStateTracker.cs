using CodeName.EventSystem.State.GameEvents;
using CodeName.EventSystem.State.Tasks;

namespace CodeName.EventSystem.State
{
    public interface IGameStateTracker<out TState> where TState : GameState
    {
        public GameEventTracker Events { get; }
        public TState State { get; }

        public StateTask RaiseEvent(GameEvent gameEvent);
    }
}
