using System.Collections.Generic;
using CodeName.Serialization;

namespace CodeName.EventSystem.GameEvents
{
    public class GameStateTrackerConfig<TGameState>
    {
        public bool IsDebugMode { get; set; } = false;
        public ISerializer Serializer { get; set; }
        public List<IGameEventHandler<TGameState>> GameEventHandlers { get; set; }
    }
}
