using System;
using CodeName.EventSystem.Tasks;

namespace CodeName.EventSystem.GameEvents
{
    public class ConstGameStateTracker<TGameState> : GameStateTracker<TGameState>
    {
        public ConstGameStateTracker(TGameState state, GameStateTrackerConfig<TGameState> config) : base(state, UseNullOpSerializer(config))
        {
            Config = config;
            Events = new GameEventTracker<TGameState>(config.Serializer);

            OriginalState = state;
            State = state;
        }

        public override async StateTask RaiseEvent(GameEvent<TGameState> gameEvent)
        {
            var currentNode = Events.Push(State, gameEvent);
            {
                await OnEventRaised(currentNode);
                currentNode.Lock();
                await OnEventConfirmed(currentNode);
                await currentNode.Event.Apply(this);
                await OnEventApplied(currentNode);
            }
            Events.Pop();
        }

        private static GameStateTrackerConfig<TGameState> UseNullOpSerializer(GameStateTrackerConfig<TGameState> config)
        {
            var result = config.ShallowCopy();
            result.Serializer = new NullOpSerializer();

            return result;
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
