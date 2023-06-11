using CodeName.EventSystem.Tasks;

namespace CodeName.EventSystem.GameEvents
{
    public class GenerativeGameStateTracker<TGameState> : GameStateTracker<TGameState>
    {
        private readonly GameStateTrackerConfig<TGameState> config;

        public GenerativeGameStateTracker(TGameState state, GameStateTrackerConfig<TGameState> config)
            : base(state, config)
        {
            this.config = config;
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

                StoreDebugExpectedState(currentNode);
            }
            Events.Pop();
        }

        private void StoreDebugExpectedState(GameEventNode<TGameState> currentNode)
        {
            if (config.IsDebugMode)
            {
                currentNode.ExpectedDebugState = config.Serializer.Clone(State);
            }
        }
    }
}
