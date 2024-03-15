using CodeName.EventSystem.GameEvents;
using CodeName.EventSystem.Tasks;

namespace CodeName.EventSystem
{
    public interface IGameStateTracker<TGameState>
    {
        public GameStateTrackerConfig<TGameState> Config { get; }
        public GameEventTracker<TGameState> Events { get; }

        public TGameState State { get; }

        public StateTask RaiseEvent(GameEvent<TGameState> gameEvent);
    }
}
