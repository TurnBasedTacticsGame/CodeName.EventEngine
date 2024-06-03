using System;
using System.Collections.Generic;
using CodeName.EventEngine.Tasks;
using CodeName.Serialization;

namespace CodeName.EventEngine.GameEvents
{
    public class ConstSimulation<TGameState> : ISimulation<TGameState>
    {
        private static NullOpSerializer Serializer = new();

        public TGameState State { get; }
        public EventTracker<TGameState> Events { get; }
        public IReadOnlyList<IGameEventHandler<TGameState>> EventHandlers { get; }

        public ConstSimulation(TGameState state, IReadOnlyList<IGameEventHandler<TGameState>> eventHandlers)
        {
            State = state;
            Events = new EventTracker<TGameState>(Serializer);
            EventHandlers = eventHandlers;
        }

        public async StateTask RaiseEvent(GameEvent<TGameState> gameEvent)
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
