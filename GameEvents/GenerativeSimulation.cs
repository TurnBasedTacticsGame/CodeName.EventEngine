using System;
using System.Collections.Generic;
using CodeName.EventEngine.Tasks;
using CodeName.Serialization;

namespace CodeName.EventEngine.GameEvents
{
    public class GenerativeSimulation<TState> : ISimulation<TState>
    {
        private readonly Config config;

        public TState State { get; }
        public EventTracker<TState> Events { get; }
        public IReadOnlyList<IEventHandler<TState>> EventHandlers => config.EventHandlers;

        public GenerativeSimulation(TState state, Config config)
        {
            this.config = config;
            State = config.Serializer.Clone(state);
            Events = new EventTracker<TState>(config.Serializer);
        }

        public async StateTask RaiseEvent(GameEvent<TState> gameEvent)
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

        private void StoreExpectedState(GameEventNode<TState> currentNode)
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
            public IReadOnlyList<IEventHandler<TState>> EventHandlers { get; set; } = Array.Empty<IEventHandler<TState>>();
        }
    }
}
