using System.Collections.Generic;
using CodeName.EventSystem.GameEvents;
using CodeName.EventSystem.Tasks;

namespace CodeName.EventSystem
{
    public class GenerativeGameStateTracker<TGameState> : GameStateTracker<TGameState>
    {
        public GenerativeGameStateTracker(TGameState state, GameStateSerializer serializer, List<IGameEventHandler<TGameState>> gameEventHandlers) : base(state, serializer, gameEventHandlers) {}

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
            if (Constants.IsDebugMode)
            {
                currentNode.ExpectedDebugState = Serializer.Clone(State);
            }
        }
    }
}
