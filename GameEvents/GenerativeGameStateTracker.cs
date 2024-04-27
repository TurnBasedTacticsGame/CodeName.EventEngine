using CodeName.EventSystem.Tasks;

namespace CodeName.EventSystem.GameEvents
{
    public class GenerativeGameStateTracker<TGameState> : GameStateTracker<TGameState>
    {
        public GenerativeGameStateTracker(TGameState state, GameStateTrackerConfig<TGameState> config) : base(state, config) {}

        public override async StateTask RaiseEvent(GameEvent<TGameState> gameEvent)
        {
            var currentNode = Events.Push(State, gameEvent);
            {
                await GameStateTrackerUtility.OnEventRaised(this, Config.GameEventHandlers);
                currentNode.Lock();
                await GameStateTrackerUtility.OnEventConfirmed(this, Config.GameEventHandlers);
                await currentNode.Event.Apply(this);
                await GameStateTrackerUtility.OnEventApplied(this, Config.GameEventHandlers);

                StoreExpectedState(currentNode);
            }
            Events.Pop();
        }

        private void StoreExpectedState(GameEventNode<TGameState> currentNode)
        {
            if (Config.IsDebugMode)
            {
                currentNode.ExpectedState = Config.Serializer.Clone(State);
            }
        }
    }
}
