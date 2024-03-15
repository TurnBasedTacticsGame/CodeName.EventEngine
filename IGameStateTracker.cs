using System.Collections.Generic;
using CodeName.EventSystem.GameEvents;
using CodeName.EventSystem.Tasks;

namespace CodeName.EventSystem
{
    public interface IGameStateTracker<TGameState>
    {
        public GameStateTrackerConfig<TGameState> Config { get; }
        
        public GameEventNode<TGameState> Tree { get; }
        public List<GameEventNode<TGameState>> List { get; }
        public List<int> PathToCurrentNode { get; }
        public GameEventNode<TGameState> CurrentNode { get; }

        public TGameState State { get; }

        public StateTask RaiseEvent(GameEvent<TGameState> gameEvent);
    }
}
