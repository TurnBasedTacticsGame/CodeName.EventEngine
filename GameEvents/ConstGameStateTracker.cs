using System;
using System.Collections.Generic;
using CodeName.EventSystem.Tasks;
using CodeName.Serialization;

namespace CodeName.EventSystem.GameEvents
{
    public class ConstGameStateTracker<TGameState> : IGameStateTracker<TGameState>
    {
        private static NullOpSerializer Serializer = new();

        public TGameState State { get; }
        public GameEventTracker<TGameState> Events { get; }
        public IEnumerable<IGameEventHandler<TGameState>> EventHandlers { get; }

        public ConstGameStateTracker(TGameState state, IEnumerable<IGameEventHandler<TGameState>> eventHandlers)
        {
            State = state;
            Events = new GameEventTracker<TGameState>(Serializer);
            EventHandlers = eventHandlers;
        }

        public async StateTask RaiseEvent(GameEvent<TGameState> gameEvent)
        {
            var currentNode = Events.Push(State, gameEvent);
            {
                await GameStateTrackerUtility.OnEventRaised(this, EventHandlers);
                currentNode.Lock();
                await GameStateTrackerUtility.OnEventConfirmed(this, EventHandlers);
                await currentNode.Event.Apply(this);
                await GameStateTrackerUtility.OnEventApplied(this, EventHandlers);
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
