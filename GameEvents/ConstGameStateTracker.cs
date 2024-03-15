using CodeName.EventSystem.Tasks;

namespace CodeName.EventSystem.GameEvents
{
    public class ConstGameStateTracker<TGameState> : GameStateTracker<TGameState>
    {
        public ConstGameStateTracker(TGameState state, GameStateTrackerConfig<TGameState> config) : base(state, config, false)
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
    }
}
