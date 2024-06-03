using System;
using System.Collections.Generic;
using CodeName.EventEngine.Tasks;
using CodeName.Serialization;

namespace CodeName.EventEngine.GameEvents
{
    public class ConstSimulation<TState> : ISimulation<TState>
    {
        private static NullOpSerializer Serializer = new();

        public TState State { get; }
        public EventTracker<TState> Events { get; }
        public IReadOnlyList<IEventHandler<TState>> EventHandlers { get; }

        public ConstSimulation(TState state, IReadOnlyList<IEventHandler<TState>> eventHandlers)
        {
            State = state;
            Events = new EventTracker<TState>(Serializer);
            EventHandlers = eventHandlers;
        }

        public async StateTask RaiseEvent(GameEvent<TState> gameEvent)
        {
            var currentNode = Events.Push(State, gameEvent);
            {
                await SimulationUtility.OnEventRaised(this, EventHandlers);
                currentNode.Lock();
                await SimulationUtility.OnEventConfirmed(this, EventHandlers);
                await currentNode.Event.Apply(this);
                await SimulationUtility.OnEventApplied(this, EventHandlers);
            }
            Events.Pop();
        }

        private class NullOpSerializer : ISerializer
        {
            public string Serialize(object value)
            {
                throw new NotSupportedException();
            }

            public T Deserialize<T>(string data)
            {
                throw new NotSupportedException();
            }

            public T Clone<T>(T value)
            {
                return value;
            }
        }
    }
}
