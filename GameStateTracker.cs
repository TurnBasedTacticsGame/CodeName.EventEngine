using System;
using CodeName.EventSystem.GameEvents;
using CodeName.EventSystem.Tasks;

namespace CodeName.EventSystem
{
    [Obsolete]
    public abstract class GameStateTracker<TGameState> : IGameStateTracker<TGameState>
    {
        protected GameStateTracker(TGameState state, GameStateTrackerConfig<TGameState> config)
        {
            Config = config;
            Events = new GameEventTracker<TGameState>(config.Serializer);

            State = config.Serializer.Clone(state);
        }

        public GameStateTrackerConfig<TGameState> Config { get; protected set; }
        public GameEventTracker<TGameState> Events { get; protected set; }

        public GameEventNode<TGameState> CurrentNode => Events.CurrentNode;

        public TGameState State { get; protected set; }

        public abstract StateTask RaiseEvent(GameEvent<TGameState> gameEvent);
    }
}
