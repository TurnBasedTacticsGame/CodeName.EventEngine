using System;
using System.Collections.Generic;
using CodeName.EventSystem.Tasks;
using CodeName.Serialization;

namespace CodeName.EventSystem.GameEvents
{
    public class GenerativeGameStateTracker<TGameState> : IGameStateTracker<TGameState>
    {
        private readonly Config config;

        public TGameState State { get; }
        public GameEventTracker<TGameState> Events { get; }
        public IReadOnlyList<IGameEventHandler<TGameState>> EventHandlers => config.EventHandlers;

        public GenerativeGameStateTracker(TGameState state, Config config)
        {
            this.config = config;
            State = config.Serializer.Clone(state);
            Events = new GameEventTracker<TGameState>(config.Serializer);
        }

        public async StateTask RaiseEvent(GameEvent<TGameState> gameEvent)
        {
            var currentNode = Events.Push(State, gameEvent);
            {
                await GameStateTrackerUtility.OnEventRaised(this, config.EventHandlers);
                currentNode.Lock();
                await GameStateTrackerUtility.OnEventConfirmed(this, config.EventHandlers);
                await currentNode.Event.Apply(this);
                await GameStateTrackerUtility.OnEventApplied(this, config.EventHandlers);

                StoreExpectedState(currentNode);
            }
            Events.Pop();
        }

        private void StoreExpectedState(GameEventNode<TGameState> currentNode)
        {
            if (config.IsDebugMode)
            {
                currentNode.ExpectedState = config.Serializer.Clone(State);
            }
        }

        public class Config
        {
            public bool IsDebugMode { get; set; } = false;
            public ISerializer Serializer { get; set; }
            public IReadOnlyList<IGameEventHandler<TGameState>> EventHandlers { get; set; } = Array.Empty<IGameEventHandler<TGameState>>();
        }
    }
}
