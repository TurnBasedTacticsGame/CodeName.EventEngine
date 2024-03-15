using System.Collections.Generic;

namespace CodeName.EventSystem.GameEvents
{
    public class GameStateTrackerConfig<TGameState>
    {
        public bool IsDebugMode { get; set; } = false;
        public bool CloneState { get; set; } = true;
        public ISerializer Serializer { get; set; }
        public List<IGameEventHandler<TGameState>> GameEventHandlers { get; set; }

        public GameStateTrackerConfig<TGameState> ShallowCopy()
        {
            return new GameStateTrackerConfig<TGameState>
            {
                IsDebugMode = IsDebugMode,
                CloneState = CloneState,
                Serializer = Serializer,
                GameEventHandlers = GameEventHandlers,
            };
        }
    }
}
