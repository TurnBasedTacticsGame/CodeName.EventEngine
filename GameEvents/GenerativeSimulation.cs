using System;
using System.Collections.Generic;
using CodeName.EventEngine.Tasks;
using CodeName.Serialization;

namespace CodeName.EventEngine.GameEvents
{
    public class GenerativeSimulation<TGameState> : ISimulation<TGameState>
    {
        private readonly Config config;

        public TGameState State { get; }
        public EventTracker<TGameState> Events { get; }
        public IReadOnlyList<IEventHandler<TGameState>> EventHandlers => config.EventHandlers;

        public GenerativeSimulation(TGameState state, Config config)
        {
            this.config = config;
            State = config.Serializer.Clone(state);
            Events = new EventTracker<TGameState>(config.Serializer);
        }

        public async StateTask RaiseEvent(GameEvent<TGameState> gameEvent)
        {
            var currentNode = Events.Push(State, gameEvent);
            {
                await SimulationUtility.OnEventRaised(this, config.EventHandlers);
                currentNode.Lock();
                await SimulationUtility.OnEventConfirmed(this, config.EventHandlers);
                await currentNode.Event.Apply(this);
                await SimulationUtility.OnEventApplied(this, config.EventHandlers);

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
            public IReadOnlyList<IEventHandler<TGameState>> EventHandlers { get; set; } = Array.Empty<IEventHandler<TGameState>>();
        }
    }
}
